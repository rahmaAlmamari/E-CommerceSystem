using System.Net;
using System.Net.Mail;
namespace E_CommerceSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.example.com"; // Replace with your SMTP server
        private readonly int _smtpPort = 587; // Replace with your SMTP port
        private readonly string _fromEmail = "noreply@yourdomain.com"; // Replace with your sender email
        private readonly string _emailPassword = "yourpassword"; // Replace with your email password

        public void SendEmail(string to, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_fromEmail, _emailPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(_fromEmail);
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                client.Send(mailMessage);
            }
        }
    }
}
