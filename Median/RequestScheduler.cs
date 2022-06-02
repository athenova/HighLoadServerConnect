using System.Diagnostics;
using Dasync.Collections;


namespace Median;

public abstract class RequestScheduler<TReq, TRes> : IRequestScheduler<TReq, TRes>
{
    public IServerConnectorBuilder ServerConnectorBuilder {  get; init; }
    public int ReadTimeoutMs { get; init; } = 10_000;
    public int ReadTimeoutDelta { get; init; } = 10_000;
    public int RetryCount { get; init; } = 20;
    public IRequestBuilder<TReq> RequestBuilder { get; init; }
    public IResponseParser<TRes> ResponseParser { get; init; }

    protected async Task<string> HandleRequest(string request, int readTimeoutMs)
    {
        var serverConnector = ServerConnectorBuilder.CreateServerConnector(readTimeoutMs);
        await serverConnector.ConnectAsync();
        await serverConnector.WriteLineAsync(request);
        return await serverConnector.ReadToEndAsync();
    }

    protected async Task<TRes> HandleRequestWithCheck(TReq request)
    {
        var readTimeout = ReadTimeoutMs;
        for (var i = 0; i < RetryCount; i++)
        {
            string key = ""; 
            try
            {
                var d = DateTime.Now;
                var requestString = RequestBuilder.Prepare(request, out key);
                var responseString = await HandleRequest(requestString, readTimeout);
                var result = ResponseParser.Parse(responseString);
                Debug.WriteLine(
                    $"{DateTime.Now}: Success: {request}, Value: {result}, Try: {i + 1}, LastElapsed: {(DateTime.Now - d).TotalMilliseconds}");
                return result;
            }
            catch (KeyHasExpiredException)
            {
                RequestBuilder.InvalidateKey(key, false);
            }
            catch (RateLimitException)
            {
                RequestBuilder.InvalidateKey(key, true);
            }
            catch
            {
                // ignored
                readTimeout += ReadTimeoutDelta;
            }
        }
        throw new Exception("Try again");
    }

    public abstract IAsyncEnumerable<TRes> HandleRequests(IEnumerable<TReq> requests);

    public static IRequestScheduler<TRequest, TResponse> CreateRequestByChunkScheduler<TRequest, TResponse>(
        IServerConnectorBuilder serverConnectorBuilder,
        int readTimeoutMs,
        int readTimeoutDelta,
        int retryCount,
        int chunkSize,
        IRequestBuilder<TRequest> requestBuilder,
        IResponseParser<TResponse> responseParser)
    {
        return new RequestByChunkScheduler<TRequest, TResponse>
        {
            ServerConnectorBuilder = serverConnectorBuilder,
            ReadTimeoutMs = readTimeoutMs,
            ReadTimeoutDelta = readTimeoutDelta,
            RetryCount = retryCount,
            ChunkSize = chunkSize,
            RequestBuilder = requestBuilder,
            ResponseParser = responseParser
        };
    }
    public static IRequestScheduler<TRequest, TResponse> CreateAdvancedRequestScheduler<TRequest, TResponse>(
        IServerConnectorBuilder serverConnectorBuilder,
        int readTimeoutMs,
        int readTimeoutDelta,
        int retryCount,
        int queueSize,
        IRequestBuilder<TRequest> requestBuilder,
        IResponseParser<TResponse> responseParser)
    {
        return new AdvancedRequestScheduler<TRequest, TResponse>
        {
            ServerConnectorBuilder = serverConnectorBuilder,
            ReadTimeoutMs = readTimeoutMs,
            ReadTimeoutDelta = readTimeoutDelta,
            RetryCount = retryCount,
            QueueSize = queueSize,
            RequestBuilder = requestBuilder,
            ResponseParser = responseParser
        };
    }
    
    public class RequestByChunkScheduler<TRequest, TResponse> : RequestScheduler<TRequest, TResponse>
    {
        public int ChunkSize { get; init; } = 10;

        public override async IAsyncEnumerable<TResponse> HandleRequests(IEnumerable<TRequest> requests)
        {
            var requestsLocal = requests.ToArray();
            var currentPointer = 0;
            var requestCount = requestsLocal.Length;
            while (currentPointer < requestCount)
            {
                foreach (var response in 
                         await Task.WhenAll(requestsLocal
                             .Skip(currentPointer)
                             .Take(ChunkSize)
                             .Select(HandleRequestWithCheck)))
                    yield return response;
                currentPointer += ChunkSize;
            }
        }
    }
    
    public class AdvancedRequestScheduler<TRequest, TResponse> : RequestScheduler<TRequest,TResponse>
    {
        public int QueueSize { get; init; } = 10;

        public override async IAsyncEnumerable<TResponse> HandleRequests(IEnumerable<TRequest> requests)
        {
            var result = new List<TResponse>();
            await requests.ParallelForEachAsync(async request => 
                { result.Add(await HandleRequestWithCheck(request)); },
                QueueSize, 
                true);
            foreach (var res in result)
                yield return res;
        }
    }
}