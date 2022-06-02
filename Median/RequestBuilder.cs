using System.Globalization;

namespace Median;

public abstract class RequestBuilder
{
    public static IRequestBuilder<string> Default { get; } = new DefaultRequestBuilder();

    protected class DefaultRequestBuilder : RequestBuilder<string>
    {
        public override string Prepare(string val, out string key)
        {
            key = "";
            return val;
        }

        public override void InvalidateKey(string oldKey, bool wait)
        {
        }
    }


    public static IRequestBuilder<int> Integer { get; } = new IntegerRequestBuilder();

    public class IntegerRequestBuilder: RequestBuilder<int>
    {
        public override string Prepare(int val, out string key)
        {
            key = "";
            return val.ToString(CultureInfo.InvariantCulture);
        }

        public override void InvalidateKey(string oldKey, bool wait)
        {
        }
    }
    public class AdvancedIntegerRequestBuilder : IntegerRequestBuilder
    {
        public IClient Client { get; init; }
        public int KeyTimeoutMs { get; init; } = 10_000; 

        private readonly object _keyMutex = new();
        private string _key;
        private bool _invalid = true;
        private bool _wait;
        private string Key
        {
            get
            {
                lock (_keyMutex)
                {
                    if (!_invalid) return _key;
                    if (_wait) Thread.Sleep(KeyTimeoutMs);
                    _wait = _invalid = false;
                    return _key = Client.GetKey().Result;
                }
            }
        }

        public override string Prepare(int val, out string key)
        {
            key = Key;
            return $"{key}|{base.Prepare(val, out _)}";
        }
        public override void InvalidateKey(string oldKey, bool wait)
        {
            lock (_keyMutex)
            {
                if (oldKey == _key)
                {
                    _invalid = true;
                    _wait = wait;
                }
            }
        }
    }
}

public abstract class RequestBuilder<T>: RequestBuilder, IRequestBuilder<T>
{
    public abstract string Prepare(T val, out string key);
    public abstract void InvalidateKey(string oldKey, bool wait);
}