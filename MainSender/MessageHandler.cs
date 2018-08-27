using Microsoft.ApplicationInsights;
using Microsoft.WindowsAzure.Storage.Queue;
using SendEmailApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MainSender
{
    public static class MessageHandler
    {
        private static readonly string AppInsightsKey = ConfigurationManager.AppSettings["instrumentalKey"];
        private static readonly TelemetryClient telemetry = new TelemetryClient { InstrumentationKey = AppInsightsKey };
        
        public static async Task<bool> ProcessMessage(CloudQueueMessage queueMsg,
            IMailSender emailVendor,
            CancellationToken cancellationToken)
        {
            const int maxDequeue = 5;

            try
            {
                // check whether a message was pulled out and returned to the queue five times
                if (queueMsg.DequeueCount >= maxDequeue)
                {
                    // if that's that case, report and return
                    telemetry.TrackTrace($"Skipping message after {maxDequeue} dequeues. " +
                        $"Message: {queueMsg?.AsString}");

                    // return 'true' in order for the message will be deleted by the Worker Role
                    return true;
                }

                var xSerilizer = new XmlSerializer(typeof(EmailDetails));
                var emailDetails = (EmailDetails)xSerilizer.Deserialize(new StringReader(queueMsg.AsString));
                return await emailVendor.SendEmail(emailDetails, telemetry, cancellationToken);

            } catch (Exception e)
            {
                //should not happen as there is a catch inside
                telemetry.TrackException(e, 
                    new Dictionary<string, string> { [emailVendor.GetType().Name] = queueMsg?.AsString });

                return false;
            }
    
        }

       
    }
}
