using System.Net;
using System.Net.Mail;
using System.Text;

namespace Gateway.Helpers.Mail;

public class EmailSender : IEmailSender
{
    public void SendEmail(string toEmail)
    {
        // Set up SMTP client
        SmtpClient client = new SmtpClient("smtp.ethereal.email", 587);
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential("rupert.oconner91@ethereal.email", "nHfT6djMazndNJWVf4");

        // Create email message
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("rupert.oconner91@ethereal.email");
        mailMessage.To.Add(toEmail);
        mailMessage.Subject = "Verification";
        mailMessage.IsBodyHtml = true;
        StringBuilder mailBody = new StringBuilder();
        mailBody.AppendFormat("<h1>You are verified</h1>");
        mailBody.AppendFormat("<br />");
        mailBody.AppendFormat("<p>Your account has been verified by an administrator.</p>");
        mailMessage.Body = mailBody.ToString();

        // Send email
        client.Send(mailMessage);
    }
}
