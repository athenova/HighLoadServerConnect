using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Median;

public class ServerConnector : IServerConnector
{
    public IPEndPoint Address { get; init; }
    public Encoding ServerEncoding { get; init; } = new Koi8rWithUtf8PreambleEncoding();
    public string WriteLineFeed { get; init; } = "\n";
    public string ReadLineFeed { get; init; } = "\r\n";
    public int ReadTimeoutMs { get; init; } = 10_000;

    public async Task ConnectAsync()
    {
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(Address);
        var stream = tcpClient.GetStream();
        stream.ReadTimeout = ReadTimeoutMs;
        _writer = new StreamWriter(stream, ServerEncoding) { NewLine = WriteLineFeed };
        _reader = new StreamReader(stream, ServerEncoding);
    }

    private StreamWriter _writer;
    private StreamReader _reader;

    public async Task WriteLineAsync(string value)
    {
        await _writer.WriteLineAsync(value);
        await _writer.FlushAsync();
    }

    public async Task<string> ReadToEndAsync()
    {
        var result = await _reader.ReadToEndAsync();
        if (!result.EndsWith(ReadLineFeed))
             throw new InvalidDataException();
        return result;
    }
}