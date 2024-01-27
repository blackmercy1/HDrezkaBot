namespace HDrezkaBot;

public class EmailData(string email, string password) : IEmail, IAbstractEmailData
{
    public string Email => email;
    public string Password => password;
    public EmailData GetEmailData()
    {
        return this;
    }
}