namespace Median;

public interface IRequestScheduler<in TReq, TRes>
{
    IAsyncEnumerable<TRes> HandleRequests(IEnumerable<TReq> requests);
}