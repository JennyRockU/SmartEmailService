using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SendEmailApi;

namespace MailgunSenderUnitTests
{
    [TestClass]
    public class MailSenderTests
    {

        [TestMethod]
        public async Task TestSendEmailWithMailgun()
        {
            var sender = new MailgunSender.Sender();
            var isSent = await MainSenderTests.TestSendEmail(sender);
            Assert.IsTrue(isSent);
        }
    }
}
