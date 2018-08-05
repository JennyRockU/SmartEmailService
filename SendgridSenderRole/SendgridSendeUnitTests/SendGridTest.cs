using System;
using System.Threading.Tasks;
using MailgunSenderUnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SendGridSenderTests
{
    [TestClass]
    public class SendGridTest
    {
        [TestMethod]
        public async Task TestSendEmailWithSendgrid()
        {
            var sender = new SendgridSender.Sender();
            var isSent = await MainSenderTests.TestSendEmail(sender);
            Assert.IsTrue(isSent);
        }
    }
}
