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
        private static readonly string[] Recipients = new string[] { "jennyrukma@gmail.com", "jennyrocku@aol.com" };
        private static readonly string SenderAddress = "john@doe.com";
        private static readonly string SubjectLine = "Test Email";
        private static readonly string EmailContent = "Hi, this email was sent via unit tests. Good luck!";

        private static readonly EmailDetails EmailDetails = new EmailDetails(Recipients,
             SenderAddress,
             SubjectLine,
              EmailContent);

        // the details' string the way it is stored in the queue
        private static readonly string WrappedEmailDetails = $"<To>{string.Join(",",Recipients)}/To>" +
            $"<From>{SenderAddress}</From>" +
            $"<Subject>{SubjectLine}</Subject>" +
            $"<Body>{EmailContent}</Body>";
        

        //Test Unwrapping Each of the Email Details
        [TestMethod]
        public void TestUnwrappingRecipients()
        {
            var expectedRecipients = string.Join(",", Recipients);
            var parsedRecipient = EmailDetails.GetTextWithTag("To", WrappedEmailDetails);

            Assert.AreEqual(expectedRecipients, parsedRecipient);
        }

        [TestMethod]
        public void TestUnwrappingSender()
        {
            var parsedSender = EmailDetails.GetTextWithTag("From", WrappedEmailDetails);

            Assert.AreEqual(SenderAddress, parsedSender);
        }

        [TestMethod]
        public void TestUnwrappingSubjectLine()
        {
            var parsedSubject = EmailDetails.GetTextWithTag("Subject", WrappedEmailDetails);

            Assert.AreEqual(SubjectLine, parsedSubject);
        }

        [TestMethod]
        public void TestUnwrappingEmailBody()
        {
            var parsedBody = EmailDetails.GetTextWithTag("Body", WrappedEmailDetails);

            Assert.AreEqual(EmailContent, parsedBody);
        }


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
