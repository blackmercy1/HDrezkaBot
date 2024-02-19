using Newtonsoft.Json;

namespace HDrezkaBot;

public class BotConfigurationProvider
{
    public string TelegramToken => _token;
    
    private string _token;
    private string _filePath;

    public BotConfigurationProvider(string filePath)
    {
        _filePath = filePath;
        _token = _filePath;
    }
}