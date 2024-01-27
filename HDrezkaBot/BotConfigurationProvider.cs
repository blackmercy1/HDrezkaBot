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
        string? jsonData = new StreamReader(_filePath).ReadToEnd();
        _token = JsonConvert.DeserializeObject<string>(jsonData) ?? throw new Exception("Invalid token read");
    }
}