using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCases
{
    [TestClass]
    public class ServicesLibTests
    {
        [TestMethod]
        public void EmailServiceTest()
        {
            IEmailSender emailSender = new AuthMessageSender();

            Assert.IsNotNull(emailSender);
        }

        [TestMethod]
        public void SMSServiceTest()
        {
            ISmsSender smsSender = new AuthMessageSender();

            Assert.IsNotNull(smsSender);
        }

        [TestMethod]
        public void UploaderServiceTest()
        {
            IUploader uploader = new UploaderService("");

            Assert.IsNotNull(uploader);
        }
    }
}
