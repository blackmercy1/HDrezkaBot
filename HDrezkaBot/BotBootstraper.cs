namespace HDrezkaBot;

public sealed class BotBootstraper : IBootstraper
{
    private readonly ConfigurationProvider _configurationProvider;

    public BotBootstraper(ConfigurationProvider configurationProvider)
    {
        _configurationProvider = configurationProvider;
    }
    
    public void Start()
    {
        var emailBootstraper = new EmailBootstraper(_configurationProvider);
        var telegramBotBootstraper = new TelegramBootstraper(emailBootstraper, _configurationProvider.TelegramTokenPath);
        telegramBotBootstraper.Start();
    }
}