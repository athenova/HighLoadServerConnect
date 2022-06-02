namespace Median;

public interface IServerConnectorBuilder
{
    IServerConnector CreateServerConnector(int readTimeoutMs);
}