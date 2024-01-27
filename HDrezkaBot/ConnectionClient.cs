namespace HDrezkaBot;

public class ConnectionClient : EmailConnectionConfigCreator
{
    protected override IConnectionData FactoryMethod(string host, int port)
    {
        return new EmailConnectionData(host, port);
    }
}

public class ConnectionDataFactory : IAbstractFactory
{
    public IAbstractConnectionData CreateConnectionData(string host, int port)
    {
        return new EmailConnectionData(host,port);
    }

    public IAbstractEmailData CreateEmailData(string email, string password = null)
    {
        return new EmailData(email, password);
    }
}

public interface IAbstractFactory
{
    IAbstractConnectionData CreateConnectionData(string host, int port);
    IAbstractEmailData CreateEmailData(string email, string password);
}

public interface IAbstractEmailData
{
    EmailData GetEmailData();
}

public interface IAbstractConnectionData : IConnectionData
{
    EmailConnectionData GetConnection();
}

public class EmailConfigurationData
{
    public Guid Id { get; set; }

    public List<IAbstractConnectionData> ConnectionData { get; set; }
    public List<IAbstractEmailData> EmailData { get; set; }
}

public class EmailConfigurationDataBuilder
{
    private EmailConfigurationData _emailConfigurationData = new EmailConfigurationData();

    public void AddEmailData(List<IAbstractEmailData> emailData)
    {
        _emailConfigurationData.EmailData = emailData;
    }

    public void AddEmailConnectionData(List<IAbstractConnectionData> connectionData)
    {
        _emailConfigurationData.ConnectionData = connectionData;
    }

    public void AddId(Guid id)
    {
        _emailConfigurationData.Id = id;
    }
}