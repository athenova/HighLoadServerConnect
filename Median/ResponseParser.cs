using System.Text.RegularExpressions;

namespace Median;

public abstract class ResponseParser
{
    public static IResponseParser<string> Default { get; } = new DefaultResponseParser();
    public static IResponseParser<int> Median { get; } = new MedianResponseParser();
    public static IResponseParser<string> Key { get; } = new KeyResponseParser();
    protected class MedianResponseParser : ResponseParser<int>
    {
        private Regex Number { get; } = new("\\d+", RegexOptions.Compiled);
        private const string KeyHasExpiredString = "Key has expired";
        public override int Parse(string st)
        {
            var match = Number.Match(st);
            if (match.Success)
                return int.Parse(match.Value);
            if (st.Contains(KeyHasExpiredString))
                throw new KeyHasExpiredException();
            throw new ArgumentException("Input string is not a number", nameof(st));
        }
    }

    protected class KeyResponseParser : DefaultResponseParser
    {
        private const string RateLimitString = "Rate limit. Please wait some time then repeat.";
        public override string Parse(string st)
        {
            if (st.Contains(RateLimitString))
                throw new RateLimitException();
            return base.Parse(st);
        }
    }

    protected class DefaultResponseParser : ResponseParser<string>
    {
        public override string Parse(string st) => st.Trim();
    }
}

public abstract class ResponseParser<T>: ResponseParser, IResponseParser<T>
{
    public abstract T Parse(string st);
}