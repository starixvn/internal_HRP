namespace HRP.Core.UnitTests
{
    using System;
    using System.IO;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class BaseMailServiceTests : IDisposable
    {
        private BaseMailService baseMailService;
        private IMailDeliver mailDeliver;

        [Fact]
        public void GetEmailDeliver_ThrowNullReferenceException()
        {
            // Arrange
            MailConfigs mailConfig = null;
            var configuration = Substitute.For<IOptionsSnapshot<MailConfigs>>();
            configuration.Value.Returns(mailConfig);

            baseMailService = Substitute.ForPartsOf<BaseMailService>(configuration);

            // Action
            Action testDelegate = () => baseMailService.GetMailDeliver();

            // Assert
            Assert.Throws<NullReferenceException>(testDelegate);
        }

        [Fact]
        public void SendEmail_MailRecordIsNull_ThrowNullReferenceException()
        {
            // Arrange
            MailRecord mailRecord = null;
            var mailConfig = new MailConfigs
            {
                AppId = 52,
                MailAddress = "net.tcp://10.18.200.116:8787/MailService",
                SenderName = "HRP"
            };
            var configuration = Substitute.For<IOptionsSnapshot<MailConfigs>>();
            configuration.Value.Returns(mailConfig);

            baseMailService = Substitute.ForPartsOf<BaseMailService>(configuration);

            // Action
            Action testDelegate = () => baseMailService.SendMail(mailRecord);

            // Assert
            Assert.Throws<NullReferenceException>(testDelegate);
        }

        [Fact]
        public void SendEmail_EmailIsNull_ReturnFalse()
        {
            // Arrange
            MailModel model = null;
            FakeBaseMailService();

            // Action
            var expected = baseMailService.SendEmail(model);

            // Assert
            Assert.False(expected);
        }

        [Fact]
        public void SendEmail_EmailNotValid_ReturnFalse()
        {
            // Arrange
            MailModel model = new MailModel
            {
                ReceiverEmail = string.Empty
            };
            FakeBaseMailService();

            // Action
            var expected = baseMailService.SendEmail(model);

            // Assert
            Assert.False(expected);
        }

        [Fact]
        public void SendEmail_EmailValid_ReturnTrue()
        {
            // Arrange
            MailModel model = GetFakeMailModel();
            FakeBaseMailService();

            // Action
            var expected = baseMailService.SendEmail(model);
            
            // Assert
            Assert.True(expected);
        }

        [Fact]
        public void SendEmail_EmailValid_CallGetEmailDeliver()
        {
            // Arrange
            MailModel model = GetFakeMailModel();
            FakeBaseMailService();

            // Action
            var expected = baseMailService.SendEmail(model);

            // Assert
            baseMailService.Received().GetEmailDeliver();
        }

        [Fact]
        public void SendEmail_EmailValid_CallSendMail()
        {
            // Arrange
            MailModel model = GetFakeMailModel();
            FakeBaseMailService();

            // Action
            var expected = baseMailService.SendEmail(model);

            // Assert
            mailDeliver.Received().SendMail(expected);
        }

        [Fact]
        public void ReadTextFile_InvalidPathFile_ThrowArgumentException()
        {
            // Arrange
            FakeBaseMailService();

            // Action
            Action testDelegate = () => baseMailService.ReadTextFile(string.Empty);

            // Assert
            Assert.Throws<ArgumentException>(testDelegate);
        }

        [Fact]
        public void ReadTextFile_InvalidPathFile_ThrowFileNotFoundException()
        {
            // Arrange
            FakeBaseMailService();

            // Action
            Action testDelegate = () => baseMailService.ReadTextFile("aaa.txt");

            // Assert
            Assert.Throws<FileNotFoundException>(testDelegate);
        }
        
        private void FakeBaseMailService()
        {
            var mailConfig = new MailConfigs
            {
                AppId = 52,
                MailAddress = "net.tcp://10.18.200.116:8787/MailService",
                SenderName = "HRP"
            };
            var configuration = Substitute.For<IOptionsSnapshot<MailConfigs>>();
            configuration.Value.Returns(mailConfig);

            mailDeliver = Substitute.For<MailDeliver>(mailConfig.AppId, mailConfig.MailAddress);
            baseMailService = Substitute.ForPartsOf<BaseMailService>(configuration);
            baseMailService.When(r => r.GetEmailDeliver()).DoNotCallBase();
            baseMailService.GetEmailDeliver().Returns(mailDeliver);

            baseMailService.When(r => r.SendEmail(Arg.Any<MailModel>())).DoNotCallBase();
        }

        private MailModel GetFakeMailModel()
        {
            MailModel model = new MailModel
            {
                ReceiverEmail = "quinn.vo@nexcel.vn",
                Subject = "Mail test",
                Body = "Mail test"
            };

            return model;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (baseMailService != null)
            {
                baseMailService.Dispose();
            }
        }
    }
}
