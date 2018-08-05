using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using MainSender;
using SendEmailApi;

namespace SendgridSender
{
    public class Sender : IMailSender
    {
            public string ApiKey => ConfigurationManager.AppSettings["apiKey"];

            public async Task<bool> SendEmail(EmailDetails email,
                TelemetryClient telelemtry,
                CancellationToken cancellationToken)
            {
                var isSucess = true;

                try
                {
                    var client = new SendGridClient(ApiKey);
                    var from = new EmailAddress(email.Sender);

                    var to = email.Recipients
                        .Select(x => new EmailAddress(x))
                        .ToList();

                    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from,
                        to, 
                        email.Subject,
                        plainTextContent: email.Body, // currently not supporting plain text
                        htmlContent: email.Body);

                    var response = await client.SendEmailAsync(msg, cancellationToken);
                    isSucess = response.StatusCode == System.Net.HttpStatusCode.Accepted;
                }
                catch (Exception e)
                {
                    telelemtry.TrackException(e,
                        new Dictionary<string, string> { { this.GetType().Name, email.Subject } });
                    isSucess = false;
                }

                return isSucess;
            }
        }
}
