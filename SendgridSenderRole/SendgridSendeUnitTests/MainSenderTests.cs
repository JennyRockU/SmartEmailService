using MainSender;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SendEmailApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailgunSenderUnitTests
{
    [TestClass]
    class MainSenderTests
    {

        // organize the wanted/expected inputs for the tests
        private static readonly List<string> Recipients = new List<string> { "jennyrukman@gmail.com", "jennyrocku@aol.com" };
        private static readonly string SenderAddress = "john@doe.com";
        private static readonly string SubjectLine = "Test Email";
        private static readonly string EmailContent = "Hi, this email was sent via unit tests. Good luck!";

        private static EmailDetails EmailDetails => EmailDetails.CreateEmail(Recipients,
                     SenderAddress,
                     SubjectLine,
                      EmailContent);

        
        // a helper method to test the sending by the given email proviser
        // using a hard-coded email details
        public static async Task<bool> TestSendEmail(IMailSender emailSender)
        {
            var isSent = await emailSender.SendEmail(EmailDetails, telelemtry: null);
            if (isSent) return true;

            //retry in case there is a temporary down time of the provider
           return await emailSender.SendEmail(EmailDetails, telelemtry: null);

        }
    }
}
