namespace Median;

public interface IResponseParser<out T>
{
    public T Parse(string st);
}