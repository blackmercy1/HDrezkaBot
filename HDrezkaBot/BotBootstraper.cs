using System.Text.RegularExpressions;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HDrezkaBot;

public sealed partial class BotBootstraper(EmailConfiguration emailConfiguration, IConnection imapclient, IConnection smtpClient)
{
    private readonly CancellationTokenSource _cancellationToken = new();
    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = Array.Empty<UpdateType>()
    };

    private readonly IConnection _smtpHost;

    private BotConfigurationProvider _botConfigurationProvider = new();
    private ITelegramBotClient _telegramBotClient;
    
    public void BotWork()
    {
        _botConfigurationProvider = GetBotConfigurationProvider();
        _telegramBotClient = GetBotClient(_botConfigurationProvider.TelegramToken);

        ExecuteEmail();
        
        _telegramBotClient.StartReceiving(
            HandleUpdateAsync, 
            HandlePollingErrorAsync, 
            _receiverOptions, 
            _cancellationToken.Token);
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        if (message.Text == "зеркало")
        {
            await botClient.SendTextMessageAsync(chatId, ExecuteEmail() ?? "string.Empty",
                cancellationToken: cancellationToken);
        }
        else
            await botClient.SendTextMessageAsync(chatId, "You said:\n" + messageText, 
                cancellationToken:cancellationToken);
    }

    private void ConnectToServer<T>(T client, IConnection connectionSettings) where T : MailService
    {
        client.Connect(connectionSettings.GetConnection().Host, connectionSettings.GetConnection().Port, true);
        client.Authenticate(emailConfiguration.EmailData.Email, emailConfiguration.EmailData.Password);
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient();
        ConnectToServer(client, smtpClient);

        return client;
    }

    private ImapClient CreateImapClient()
    {
        var client = new ImapClient();
        ConnectToServer(client, imapclient);
        client.Inbox.Open(FolderAccess.ReadOnly);
        
        return client;
    }

    private IEnumerable<UniqueId> GetAllEmails(ImapClient client)
    {
        var emailMessages = client.Inbox.Search(SearchQuery.All).Reverse();
        return emailMessages;
    }

    private MimeMessage CreateMessage()
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("Gay", emailConfiguration.EmailData.Email));
        mailMessage.To.Add(new MailboxAddress("gayb", emailConfiguration.TargetEmail));
        mailMessage.Body = new TextPart
        {
            Text = "mirror"
        };

        return mailMessage;
    }

    private string ExecuteEmail()
    {
        var imapClient = CreateImapClient();
        var smtpClient = CreateSmtpClient();

        var mimeMessage = CreateMessage();

        smtpClient.Send(mimeMessage);
        
        var emailMessages = GetAllEmails(imapClient);
        
        foreach (var email in emailMessages)
        {
            var message = imapClient.Inbox.GetMessage(email);
            var messageSender = DeleteUselessFragment(message.From.ToString() ?? string.Empty);

            if (messageSender == emailConfiguration.TargetEmail)
            {
                var messageBody = message.HtmlBody;
                foreach (var url in MyRegex().Matches(messageBody))
                {
                    var matchUrl = url as Match;
                    if (matchUrl is not null && matchUrl.Value.Contains("hdrezka"))
                        return matchUrl.Value;  
                }
            }
        }

        return string.Empty;
    }

    private string DeleteUselessFragment(string message)
    {
        var startIndex= 0;
        var endIndex = 0;
        
        for (var i = 0; i < message.Length; i++)
        {
            if (message[i] == '<')
                startIndex = i + 1;
            if (message[i] == '>')
                endIndex = i;
        }
        
        var wordLenght = endIndex - startIndex;
        message = message.Substring(startIndex, wordLenght);
        return message;
    }
    
    private BotConfigurationProvider GetBotConfigurationProvider()
    {
        return _botConfigurationProvider == null ? _botConfigurationProvider = new BotConfigurationProvider() :_botConfigurationProvider;
    }

    private ITelegramBotClient GetBotClient(string token)
    {
        return _telegramBotClient == null ? _telegramBotClient = new TelegramBotClient(token) : _telegramBotClient;
    }

    [GeneratedRegex(@"(http|ftp|https):\/\/([\w\-_]+(?:\.[\w\-_]+)+)([\w\-\.,@?^=%&amp;~\+#]*[\w\-\@?^=%&amp;/~\+#])?")]
    private static partial Regex MyRegex();
}