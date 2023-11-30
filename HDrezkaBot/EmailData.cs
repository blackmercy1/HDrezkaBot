namespace HDrezkaBot;

public class EmailData(string email, string password) : IEmail
{
    public string Email => email;
    public string Password => password;
}