using Blaster.Infrastructure.Utility.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace Blaster.Infrastructure.Utility
{
    public class EmailHelper : IEmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Render(string to, string subject, string template, object data)
        {
            var html = ManifestDataReader.Get<EmailHelper>(template, data);

            Send(to, subject, html);
        }

        public void Send(string to, string subject, string body)
        {
            var smtpConfig = _configuration.GetSection("Smtp");

            if (!Convert.ToBoolean(smtpConfig["Enabled"]))
            {
                return;
            }

            using (var smtpClient = new SmtpClient(smtpConfig["Host"], int.Parse(smtpConfig["Port"]))
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(smtpConfig["User"], smtpConfig["Password"]),
                EnableSsl = Convert.ToBoolean(smtpConfig["EnableSsl"])
            })
            using (var mailMessage = new MailMessage()
            {
                From = new MailAddress(smtpConfig["From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            })
            {
                mailMessage.To.Add(to);
                smtpClient.Send(mailMessage);
            }
        }


    }
}
