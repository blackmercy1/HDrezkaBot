namespace HDrezkaBot;

public abstract class EmailConnectionConfigCreator
{
    protected abstract IConnection FactoryMethod(string host, int port);

    public IConnection GetConnection(string host, int port)
    {
        var connection = FactoryMethod(host, port);
        return connection;
    }
}