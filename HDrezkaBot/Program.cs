namespace HDrezkaBot;

public class Program
{
    public static void Main(string[] args)
    {
        var clientFactory = new ConnectionClient();
        var imapclient = clientFactory.GetConnection("imap.gmail.com", 993);
        var smtpClient = clientFactory.GetConnection("smtp.gmail.com",465);
        var emailConfiguration = new EmailConfiguration
        ("", 
            "", 
            "mirror@hdrezka.org" );
        
        var bootstraper = new BotBootstraper(emailConfiguration, imapclient, smtpClient);
        
        bootstraper.BotWork();
        while (true)
        {
        }
    }
}