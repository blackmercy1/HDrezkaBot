namespace HDrezkaBot;

public class EmailConnectionData(string host, int port) : IAbstractConnectionData
{
    public string Host => host;
    public int Port => port;
    
    public EmailConnectionData GetConnection()
    {
        return this;
    }
}

public class SelimGay(string gay)
{
    private void SelimPedik()
    {

    }
}

public record MegaGay(string pop);