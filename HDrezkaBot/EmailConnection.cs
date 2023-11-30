namespace HDrezkaBot;

public class EmailConnection(string host, int port) : IConnection
{
    public string Host => host;
    public int Port => port;
    
    public EmailConnection GetConnection()
    {
        return this;
    }
}