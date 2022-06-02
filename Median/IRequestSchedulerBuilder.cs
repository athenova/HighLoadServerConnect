namespace Median;

public interface IRequestSchedulerBuilder
{
    IRequestScheduler<TReq, TRes> GetConnector<TReq, TRes>(IRequestBuilder<TReq> requestBuilder,
        IResponseParser<TRes> responseParser);
}