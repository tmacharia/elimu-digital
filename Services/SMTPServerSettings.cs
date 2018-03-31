using System;
using System.Linq;

namespace Services
{
    public class SMTPServerSettings
    {
        public string Host { get; set; }
        public string Authentication { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }

        public SMTPServerSettings GetSMTPServer(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email");
            }

            string[] tokens = email.Split('@');
            string name = tokens.First();
            string host = tokens.Last().Split('.').First();

            if (host == "gmail")
            {
                return new SMTPServerSettings()
                {
                    Host = "smtp.gmail.com",
                    UserName = name,
                    Authentication = "TLS",
                    Port = 587,
                };
            }
            else if (host == "outlook")
            {
                return new SMTPServerSettings()
                {
                    Host = "smtp-mail.outlook.com",
                    UserName = name,
                    Authentication = "SSL",
                    Port = 587,
                };
            }
            else if(host == "live" || host == "hotmail")
            {
                return new SMTPServerSettings()
                {
                    Host = "smtp.live.com",
                    UserName = name,
                    Authentication = "SSL",
                    Port = 587,
                };
            }
            else if(host == "yahoo")
            {
                return new SMTPServerSettings()
                {
                    Host = "smtp.mail.yahoo.com",
                    UserName = name,
                    Authentication = "SSL",
                    Port = 465,
                };
            }
            else
            {
                return new SMTPServerSettings()
                {
                    Host = "smtp.zoho.com",
                    Authentication = "SSL",
                    UserName = name,
                    Port = 465,
                };
            }
        }
    }
}
