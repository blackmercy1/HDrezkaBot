namespace HDrezkaBot;

public sealed class EmailParser()
{
    public string FragmentateEmail(string message)
    {
        var startIndex = 0;
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
    
}