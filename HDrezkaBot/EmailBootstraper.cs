using System.Text.RegularExpressions;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;

namespace HDrezkaBot;

public partial class EmailBootstraper : IEmailBootstraper
{
    private readonly IConnectionData _imapclientData;
    private readonly IConnectionData _smtpClientData;
    private readonly IEmail _signInData;
    private readonly IEmail _targetEmail;

    public EmailBootstraper(ConfigurationProvider configurationProvider)
    {
        _targetEmail = configurationProvider.TargetEmail.GetEmailData();
        _signInData = configurationProvider.SignInData.GetEmailData();
        _imapclientData = configurationProvider.ImapClient.GetConnection();
        _smtpClientData = configurationProvider.SmtpClient.GetConnection();
    }

    public string TryGetLink()
    {
        SendEmail();
        return ReceiveEmailLink();
    }

    private void ConnectToServer<T>(T client, IConnectionData connectionDataSettings) where T : MailService
    {
        client.Connect(connectionDataSettings.GetConnection().Host, connectionDataSettings.GetConnection().Port, true);
        client.Authenticate(_signInData.Email, _signInData.Password);
    }

    private void DisconnectFromServer<T>(T client) where T : MailService
    {
        client.Disconnect(true);
        client.Dispose();
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient();
        ConnectToServer(client, _smtpClientData);

        return client;
    }

    private ImapClient CreateImapClient()
    {
        var client = new ImapClient();
        ConnectToServer(client, _imapclientData);
        client.Inbox.Open(FolderAccess.ReadOnly);

        return client;
    }

    private IEnumerable<UniqueId> GetAllEmailsId(ImapClient client)
    {
        var emailMessages = client.Inbox.Search(SearchQuery.All).Reverse();
        return emailMessages;
    }

    private MimeMessage CreateMessage()
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("Gay", _signInData.Email));
        mailMessage.To.Add(new MailboxAddress("gay", _targetEmail.Email));
        mailMessage.Body = new TextPart
        {
            Text = "mirror"
        };

        return mailMessage;
    }

    private void SendEmail()
    {
        var smtpClient = CreateSmtpClient();

        var mimeMessage = CreateMessage();

        smtpClient.SendWithAwait(smtpClient, mimeMessage, 5000);
        DisconnectFromServer(smtpClient);
    }

    private string ReceiveEmailLink()
    {
        var imapClient = CreateImapClient();
        var link = SearchTargetEmailInbox(imapClient);

        DisconnectFromServer(imapClient);

        return link == string.Empty ? "Nothing was found" : link;
    }

    private string SearchTargetEmailInbox(ImapClient imapClient)
    {
        var emailsId = GetAllEmailsId(imapClient);
        
        foreach (var id in emailsId)
        {
            var message = imapClient.Inbox.GetMessage(id);
            var emailParser = new EmailParser();
            var messageSenderEmail = emailParser.FragmentateEmail(message.From.ToString() ?? string.Empty);
            
            if (messageSenderEmail == string.Empty)
                continue;
            
            if (CheckTargetEmail(messageSenderEmail))
            {
                return GetLinkFromEmail(message);
            }
        }

        return string.Empty;
    }

    private string GetLinkFromEmail(MimeMessage message)
    {
        var messageBody = message.HtmlBody;
        foreach (var url in MyRegex().Matches(messageBody))
            if (url is Match matchUrl && matchUrl.Value.Contains("hdrezka"))
                return matchUrl.Value;
        return string.Empty;
    }

    private bool CheckTargetEmail(string email)
    {
        return email == _targetEmail.Email;
    }
    
    [GeneratedRegex(@"(http|ftp|https):\/\/([\w\-_]+(?:\.[\w\-_]+)+)([\w\-\.,@?^=%&amp;~\+#]*[\w\-\@?^=%&amp;/~\+#])?")]
    private static partial Regex MyRegex();

    public void Start()
    {
        
    }
}