using HDrezkaBot;

public class ConfigurationProvider
{
    public IAbstractConnectionData ImapClient;
    public IAbstractConnectionData SmtpClient;
    public IAbstractEmailData SignInData;
    public IAbstractEmailData TargetEmail;

    public string TelegramTokenPath;
    
    private ConnectionDataFactory _dataFactory;
    
    public ConfigurationProvider()
    {
        _dataFactory = new ConnectionDataFactory();
        
        ImapClient = _dataFactory.CreateConnectionData("imap.gmail.com", 993);
        SmtpClient = _dataFactory.CreateConnectionData("smtp.gmail.com", 465);
        
        SignInData = _dataFactory.CreateEmailData("blackmercy228@gmail.com", "pufj lvwi mrte rygt");
        TargetEmail = _dataFactory.CreateEmailData("mirror@hdrezka.org");  
        
        TelegramTokenPath = "6371141732:AAEy3ictmLnfU8NzqBvmNWv0gOznG0jAT1E";
    }
}