using MailKit.Net.Smtp;
using MimeKit;

namespace HDrezkaBot;

public static class SmtpClientExtension
{
    public static void SendWithAwait(this SmtpClient _, SmtpClient client,MimeMessage mimeMessage, int timeToAwait)
    {
        client.Send(mimeMessage);
        Thread.Sleep(timeToAwait);
    }
}