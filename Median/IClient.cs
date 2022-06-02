namespace Median;

public interface IClient
{
    Task<string> GetTask();
    Task<float> GetMedian(IEnumerable<int> requests);
    Task<string> CheckAnswer(float number);
    Task<string> GetKey();
    Task<float> GetAdvancedMedian(IEnumerable<int> requests);
    Task<string> CheckAdvancedAnswer(float number);
}