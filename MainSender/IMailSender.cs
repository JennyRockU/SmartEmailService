using Microsoft.ApplicationInsights;
using SendEmailApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MainSender
{
    public interface IMailSender
    {
        string ApiKey { get; }
        Task<bool> SendEmail(EmailDetails msg,
            TelemetryClient telelemtry,
            CancellationToken cancelation = new CancellationToken());
    }

    public class EmailMessage
    {
        public EmailMessage(KeyValuePair<string, string> sender,
            Dictionary<string, string> recipeints,
            string subject,
            string body)
        {
            this.Sender = sender;
            this.Recipeints = recipeints;
            this.Subject = subject;
            this.Body = body;
        }

        public KeyValuePair<string, string> Sender { get; set; }
        public Dictionary<string, string> Recipeints { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
    }
}