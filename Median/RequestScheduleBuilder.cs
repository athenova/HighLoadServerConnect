namespace Median;

public abstract class RequestScheduleBuilder
    : IRequestSchedulerBuilder
{
    public IServerConnectorBuilder ServerConnectorBuilder { get; init; }  
    public int ReadTimeoutMs { get; init; } = 10_000;
    public int ReadTimeoutDelta { get; init; } = 10_000;
    public int RetryCount { get; init; } = 20;
    public abstract IRequestScheduler<TReq, TRes> GetConnector<TReq, TRes>(IRequestBuilder<TReq> requestBuilder, IResponseParser<TRes> responseParser);

    public static IRequestSchedulerBuilder CreateRequestByChunkSchedulerBuilder(
        IServerConnectorBuilder serverConnectionBuilder,
        int chunkSize = 50,
        int retryCount = 20,
        int readTimeoutMs = 10_000,
        int readTimeoutDelta = 10_000)
    {
        return new RequestByChunkSchedulerBuilder()
        {
            ServerConnectorBuilder = serverConnectionBuilder,
            ChunkSize = chunkSize,
            RetryCount = retryCount,
            ReadTimeoutMs = readTimeoutMs,
            ReadTimeoutDelta = readTimeoutDelta
        };
    }
    
    public static IRequestSchedulerBuilder CreateAdvancedRequestSchedulerBuilder(
        IServerConnectorBuilder serverConnectionBuilder,
        int queueSize = 50,
        int retryCount = 20,
        int readTimeoutMs = 10_000,
        int readTimeoutDelta = 10_000)
    {
        return new AdvancedSchedulerBuilder()
        {
            ServerConnectorBuilder = serverConnectionBuilder,
            QueueSize = queueSize,
            RetryCount = retryCount,
            ReadTimeoutMs = readTimeoutMs,
            ReadTimeoutDelta = readTimeoutDelta
        };
    }
    
    public class RequestByChunkSchedulerBuilder: RequestScheduleBuilder
    {   
        public int ChunkSize { get; init; } = 50;
    
        public override IRequestScheduler<TReq, TRes> GetConnector<TReq, TRes>
        (
            IRequestBuilder<TReq> requestBuilder,
            IResponseParser<TRes> responseParser) 
            => RequestScheduler<TReq, TRes>.CreateRequestByChunkScheduler(
                ServerConnectorBuilder,
                ReadTimeoutMs,
                ReadTimeoutDelta,
                RetryCount,
                ChunkSize,
                requestBuilder,
                responseParser
            );
    }
    
    public class AdvancedSchedulerBuilder: RequestScheduleBuilder
    {   
        public int QueueSize { get; init; } = 50;
    
        public override IRequestScheduler<TReq, TRes> GetConnector<TReq, TRes>
        (
            IRequestBuilder<TReq> requestBuilder,
            IResponseParser<TRes> responseParser) 
            => RequestScheduler<TReq, TRes>.CreateAdvancedRequestScheduler(
                ServerConnectorBuilder,
                ReadTimeoutMs,
                ReadTimeoutDelta,
                RetryCount,
                QueueSize,
                requestBuilder,
                responseParser
            );
    }
}