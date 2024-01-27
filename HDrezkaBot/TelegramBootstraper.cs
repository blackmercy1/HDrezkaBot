using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HDrezkaBot;

public sealed class TelegramBootstraper : IBootstraper
{
    private readonly IEmailBootstraper _emailBootstraper;
    private readonly CancellationTokenSource _cancellationToken = new();

    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = Array.Empty<UpdateType>()
    };

    private BotConfigurationProvider _botConfigurationProvider;
    private ITelegramBotClient _telegramBotClient;
    private readonly string _telegramTokenPath;
    
    public TelegramBootstraper(IEmailBootstraper emailBootstraper, string telegramTokenPath)
    {
        _telegramTokenPath = telegramTokenPath;
        _emailBootstraper = emailBootstraper;
    }

    private void SetupTelegramBot()
    {
        _botConfigurationProvider = GetBotConfigurationProvider();
        _telegramBotClient = GetBotClient(_botConfigurationProvider.TelegramToken);

        _telegramBotClient.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            _receiverOptions,
            _cancellationToken.Token);
    }
    
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
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

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, 
        CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        if (message.Text == "зеркало")
        {
            await botClient.SendTextMessageAsync(chatId, _emailBootstraper.TryGetLink() ?? "string.Empty",
                cancellationToken: cancellationToken);
        }
        else
            await botClient.SendTextMessageAsync(chatId, "You said:\n" + messageText,
                cancellationToken: cancellationToken);
    }
    
    private BotConfigurationProvider GetBotConfigurationProvider()
    {
        return _botConfigurationProvider == null
            ? _botConfigurationProvider = new BotConfigurationProvider(_telegramTokenPath)
            : _botConfigurationProvider;
    }

    private ITelegramBotClient GetBotClient(string token)
    {
        return _telegramBotClient == null ? _telegramBotClient = new TelegramBotClient(token) : _telegramBotClient;
    }

    public void Start()
    {
        SetupTelegramBot();
    }
}