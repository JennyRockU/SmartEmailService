using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SendEmailApi;

namespace SendEmailApiUnitTests
{
    [TestClass]
    public class EmailDetailsTests
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

        // the details' string the way it is stored in the queue
        private static readonly string EmailDetailsStr = $"<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            $"<EmailDetails xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
            $"<Recipients><string>jennyrukman@gmail.com</string><string>jennyrocku@aol.com</string></Recipients>" +
            $"<Sender>john@doe.com</Sender>" +
            $"<Subject>Test Email</Subject>" +
            $"<Body>Hi, this email was sent via unit tests. Good luck!</Body>" +
            $"</EmailDetails>";

        [TestMethod]
        public void TestSerialization()
        {
            var serializedemailString = EmailHandler.Serialize(EmailDetails);
            Assert.AreEqual(EmailDetailsStr, serializedemailString);
        }

        [TestMethod]
        public void TestRecipientsValidation()
        {
            // valid email list
            Assert.IsTrue(EmailDetails.ValidRecipients);

            // invalid emails' amount
            const int restrictedNoOfEmails = 52;
            var lengthyRecipientList = Enumerable.Repeat("testemail@gmail.com", restrictedNoOfEmails).ToList();
            var faultyRecipeintsEmail = EmailDetails.CreateEmail(lengthyRecipientList, SenderAddress, SubjectLine, EmailContent);
            Assert.IsFalse(faultyRecipeintsEmail.ValidRecipients);

            // invalid adddress
            var badEmailFormatRecipientList = new List<string> { "jennygmail.com" };
            var faultyRecipeintAddressEmail = EmailDetails.CreateEmail(badEmailFormatRecipientList, SenderAddress, SubjectLine, EmailContent);
            Assert.IsFalse(faultyRecipeintAddressEmail.ValidRecipients);

        }
        
       
    }

    [TestClass]
    public class ApiCallAuthorizationTests
    {
        const string existingToken = "MailApiTestTokenString123";
        const string nonExistingToken = "MailApiTestToken123False";

        [TestMethod]
        public void TestValidToken()
        {
            Assert.IsTrue(Authentication.VaidateUser(existingToken));
        }

        [TestMethod]
        public void TestInValidToken()
        {
            Assert.IsFalse(Authentication.VaidateUser(""));
            Assert.IsFalse(Authentication.VaidateUser(nonExistingToken));
        }
    }
}
