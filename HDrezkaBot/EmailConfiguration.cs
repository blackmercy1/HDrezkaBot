namespace HDrezkaBot;

public class EmailConfiguration
{
    public readonly string TargetEmail;
    public readonly IEmail EmailData;

    public EmailConfiguration(string email, string password, string targetEmail)
    {
        EmailData = new EmailData(email, password);
        TargetEmail = new EmailData(targetEmail, "").Email;
    }
}