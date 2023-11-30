namespace HDrezkaBot;

public class ConnectionClient : EmailConnectionConfigCreator
{
    protected override IConnection FactoryMethod(string host, int port)
    {
        return new EmailConnection(host, port);
    }
}