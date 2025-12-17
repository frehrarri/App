using MailKit.Net.Smtp;
using MimeKit;
using Voyage.Models;

namespace Voyage.Services
{
    public class EmailService
    {
        public async Task Send(Email email)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(email.FromName, email.FromEmail));
            message.To.Add(new MailboxAddress(email.ToName, email.ToEmail));
            message.Subject = email.Subject;
            message.Body = new TextPart("plain")
            {
                Text = email.Body
            };

            using (var client = new SmtpClient())
            {
                // connect
                //Replace the SMTP server + creds with your real ones (Gmail, Outlook, your domain, whatever).
                await client.ConnectAsync("smtp.yourmailserver.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // authenticate
                await client.AuthenticateAsync("you@example.com", "yourpassword");

                // send it
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
