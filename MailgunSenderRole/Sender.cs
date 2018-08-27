using MainSender;
using Microsoft.ApplicationInsights;
using RestSharp;
using RestSharp.Authenticators;
using SendEmailApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MailgunSender
{
    public class Sender : IMailSender
    {
        public string ApiKey => ConfigurationManager.AppSettings["apiKey"];
        private readonly string BaseUrl = "https://api.eu.mailgun.net/v3";
        private readonly string SandboxDomain = "sandbox7991e4e8835c480d858e978b822ef530.mailgun.org";
        private readonly string AppDomain = "smartemailservice.azurewebsites.net";

        public async Task<bool> SendEmail(EmailDetails email,
            TelemetryClient telelemtry,
            CancellationToken cancellationToken)
        {
            var isSucess = true;
            try
            {
                var client = new RestClient
                {
                    BaseUrl = new Uri(BaseUrl),
                    Authenticator = new HttpBasicAuthenticator("api", ApiKey)
                };

                var request = new RestRequest();
                request.AddParameter("domain", AppDomain, ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", $"{email.Sender} <mailgun@{AppDomain}>");
                request.AddParameter("subject", email.Subject);
                request.AddParameter("text", email.Body);

                foreach(var recipient in email.Recipients)
                {
                    request.AddParameter("to", recipient);
                }

                request.Method = Method.POST;
                var res = await client.ExecuteTaskAsync<string>(request);

                return res.IsSuccessful;
            }
            catch (Exception e)
            {

                telelemtry.TrackException(e,
                    new Dictionary<string, string> { [this.GetType().Name] = email.Subject } );
                isSucess = false;
            }

            return isSucess;
        }

        private bool SendViaSandBox(EmailDetails email)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri("https://api.mailgun.net/v3"),
                Authenticator = new HttpBasicAuthenticator("api", ApiKey)
            };

            var request = new RestRequest();
            request.AddParameter("domain", SandboxDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@sandbox7991e4e8835c480d858e978b822ef530.mailgun.org>");
            request.AddParameter("subject", email.Subject);
            request.AddParameter("text", email.Body);

            foreach (var recipient in email.Recipients)
            {
                request.AddParameter("to", recipient);
            }

            request.Method = Method.POST;
            var res = client.Execute(request);
            return res.IsSuccessful;
        }

    }
}
