using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SendEmailApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmailsController : ApiController
    {
        private static string AppInsightsKey = WebConfigurationManager.AppSettings["appInsightsInstrumentalKey"];
        private static readonly TelemetryClient telemetry = new TelemetryClient { InstrumentationKey = AppInsightsKey };

        // POST api/emails
        [HttpPost]
        public async Task<IHttpActionResult> SendEmail(
            string to,
            string from,
            string subject)
        {
            var content = string.Empty;
            try
            {
                content = await Request.Content.ReadAsStringAsync();

            } catch (Exception e)
            {
                telemetry.TrackException(e);
                return BadRequest("Failed to get email content");
            }

            if(String.IsNullOrEmpty(to)) return BadRequest("Recipient email is missing (to)");
            var emailDetails = new EmailDetails(to.Split(','), from, subject, content);
            
            if (!emailDetails.ValidRecipients) return BadRequest("Recipient email is either missing or not in a correct format");
            if (!emailDetails.ValidSender) return BadRequest("Sender email is either missing or not in a correct format");
            if (!emailDetails.ValidSubject) return BadRequest("Title exceeds allowed length");
        
            try
            {
                 await EmailHandler.SendEmailToQueue(emailDetails);
            } catch
            {
                //retry
                await EmailHandler.SendEmailToQueue(emailDetails);
            }

            return Ok("Email created");
        }
        
    }
}
