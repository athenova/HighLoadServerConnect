namespace Median;

public interface IRequestBuilder<in T>
{
    string Prepare(T val, out string key);
    void InvalidateKey(string oldKey, bool wait);
}