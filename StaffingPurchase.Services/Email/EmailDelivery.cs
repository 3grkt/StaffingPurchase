using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using StaffingPurchase.Core;

namespace StaffingPurchase.Services.Email
{
    public class EmailDelivery : IEmailDelivery
    {
        private readonly IAppSettings _appSettings;

        public EmailDelivery(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public void Send(string subject, string body, string to, string from)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = _appSettings.SmtpClientEmailFrom;
            }
            MailMessage mailMessage = new MailMessage(from,
                to)
            { IsBodyHtml = true };
            SmtpClient smtpClient = new SmtpClient
            {
                Port = _appSettings.SmtpClientPort,
                Host = _appSettings.SmtpClientHost,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = _appSettings.SmtpClientEnableSsl,
                Timeout = _appSettings.SmtpClientTimeout,
                Credentials = new NetworkCredential(_appSettings.SmtpClientUser, _appSettings.SmtpClientPassword)
            };
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            if (_appSettings.TurnOffCertificateValidation)
            {
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
            }

            smtpClient.Send(mailMessage);
        }
    }
}
