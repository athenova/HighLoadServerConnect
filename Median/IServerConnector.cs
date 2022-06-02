namespace Median;

public interface IServerConnector
{
    Task ConnectAsync();
    Task WriteLineAsync(string value);
    Task<string> ReadToEndAsync();
}