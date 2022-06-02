namespace Median;

public class Client : IClient
{
    public IRequestSchedulerBuilder SchedulerBuilder { get; init; }

    public async Task<string> GetTask()
    {
        const string request = "Greetings";
        var connector = SchedulerBuilder.GetConnector(RequestBuilder.Default, ResponseParser.Default);
        var sequence = connector.HandleRequests(new []{ request });
        return await sequence.FirstAsync();
    }
    
    public async Task<float> GetMedian(IEnumerable<int> requests)
    {
        var connector = SchedulerBuilder.GetConnector(RequestBuilder.Integer, ResponseParser.Median); 
        var sequence = await connector.HandleRequests(requests).ToArrayAsync();
        Array.Sort(sequence);
        return sequence.Length % 2 != 0
            ? sequence[sequence.Length / 2]
            : (sequence[sequence.Length / 2 - 1] + sequence[sequence.Length / 2]) / 2f;
    }

    public async Task<string> CheckAnswer(float number)
    {
        var request = $"Check {number}";
        var connector = SchedulerBuilder.GetConnector(RequestBuilder.Default, ResponseParser.Default);
        var sequence = connector.HandleRequests(new []{ request });
        return await sequence.FirstAsync();
    }

    public async Task<string> GetKey()
    {
        const string request = "Register";
        var connector = SchedulerBuilder.GetConnector(RequestBuilder.Default, ResponseParser.Key);
        var sequence = connector.HandleRequests(new []{ request });
        return await sequence.FirstAsync();
    }
    
    public async Task<float> GetAdvancedMedian(IEnumerable<int> requests)
    {
        var connector = SchedulerBuilder.GetConnector
        (
            new RequestBuilder.AdvancedIntegerRequestBuilder { Client = this, KeyTimeoutMs = 10_0000 }, 
            ResponseParser.Median
            ); 
        var sequence = await connector.HandleRequests(requests).ToArrayAsync();
        Array.Sort(sequence);
        return sequence.Length % 2 != 0
            ? sequence[sequence.Length / 2]
            : (sequence[sequence.Length / 2 - 1] + sequence[sequence.Length / 2]) / 2f;
    }
    
    public async Task<string> CheckAdvancedAnswer(float number)
    {
        var request = $"Check_advanced {number}";
        var connector = SchedulerBuilder.GetConnector(RequestBuilder.Default, ResponseParser.Default);
        var sequence = connector.HandleRequests(new []{ request });
        return await sequence.FirstAsync();
    } 
}