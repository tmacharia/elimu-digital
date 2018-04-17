using MailKit.Net.Smtp;
using MimeKit;
using Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Security;
using MimeKit.Text;
using System.Linq;

namespace Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly SMTPServerSettings settings = new SMTPServerSettings();
        private readonly string Title;
        private readonly string mail;
        private readonly string _password;
        public string Footer
        {
            get
            {
                return $"<br/><br/>Cheers!<br/>{Title} Team.";
            }
        }
        public AuthMessageSender()
        {

        }
        public AuthMessageSender(string title,string email,string password)
        {
            Title = title;
            mail = email;
            _password = password;
        }

        public async Task SendEmailAsync(string subject, string message, params string[] emails)
        {
            try
            {
                var mime = new MimeMessage();

                mime.From.Add(new MailboxAddress(Title, mail));

                for (int i = 0; i < emails.Length; i++)
                {
                    mime.To.Add(new MailboxAddress(emails[i]));
                    mime.Body = new TextPart(TextFormat.Html)
                    {
                        Text = $"Hello {emails[i].Split('@').First()},<br/>{message}{Footer}"
                    };
                }

                mime.Subject = subject;

                using (var client = new SmtpClient())
                {
                    //client.LocalDomain = "e-learningpad.com";
                    var smtp = settings.GetSMTPServer(mail);


                    await client.ConnectAsync(smtp.Host, smtp.Port, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(mail, _password);

                    await client.SendAsync(mime).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
