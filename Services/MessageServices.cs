using MailKit.Net.Smtp;
using MimeKit;
using Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Security;
using MimeKit.Text;

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

        public AuthMessageSender()
        {

        }
        public AuthMessageSender(string title,string email,string password)
        {
            Title = title;
            mail = email;
            _password = password;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mime = new MimeMessage();

                mime.From.Add(new MailboxAddress(Title, mail));
                mime.To.Add(new MailboxAddress("", email));
                mime.Subject = subject;
                mime.Body = new TextPart(TextFormat.Html) { Text = message + Footer };

                using (var client = new SmtpClient())
                {
                    client.LocalDomain = /*(Helpers.IsDebug) ? "localhost" : */"e-learningpad.com";
                    var smtp = settings.GetSMTPServer(mail);


                    await client.ConnectAsync(smtp.Host, 25, SecureSocketOptions.StartTls).ConfigureAwait(false);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(smtp.UserName, _password);

                    await client.SendAsync(mime).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
            }
            catch (System.Exception)
            {

            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

        #region Private Section
        private string Footer
        {
            get
            {
                return $"<br/><br/>Thank you,<br/> {Title} Support Team.";
            }
        }
        #endregion
    }
}
