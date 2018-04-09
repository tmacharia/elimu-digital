using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string subject, string message, params string[] emails);
    }
}
