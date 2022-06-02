using System.Net;
using System.Text;

namespace Median;

public class ServerConnectorBuilder
    : IServerConnectorBuilder
{
    public IPEndPoint Address { get; init; }
    public Encoding ServerEncoding { get; init; } = new Koi8rWithUtf8PreambleEncoding();
    public string WriteLineFeed { get; init; } = "\n";
    public string ReadLineFeed { get; init; } = "\r\n";
    public IServerConnector CreateServerConnector(int readTimeoutMs)
    {
        return new ServerConnector
        {
            Address = Address,
            ServerEncoding = ServerEncoding,
            ReadLineFeed = ReadLineFeed,
            WriteLineFeed = WriteLineFeed,
            ReadTimeoutMs = readTimeoutMs
        };
    }
}