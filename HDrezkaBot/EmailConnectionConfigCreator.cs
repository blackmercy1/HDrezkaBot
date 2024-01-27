namespace HDrezkaBot;

public abstract class EmailConnectionConfigCreator
{
    protected abstract IConnectionData FactoryMethod(string host, int port);

    public IConnectionData GetConnection(string host, int port)
    {
        var connection = FactoryMethod(host, port);
        return connection;
    }
}