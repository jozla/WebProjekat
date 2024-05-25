namespace Gateway.Helpers.Mail;

public interface IEmailSender
{
    void SendEmail(string toEmail);
}
