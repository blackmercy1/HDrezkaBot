using HDrezkaBot;

var configurationProvider = new ConfigurationProvider();
IBootstraper bootstraper = new BotBootstraper(configurationProvider);
bootstraper.Start();
        
while (true)
{
}