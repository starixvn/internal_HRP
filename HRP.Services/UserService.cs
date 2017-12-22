namespace HRP.Services
{
    using BCrypt.Net;
    using Core.Constants;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Microsoft.Extensions.Options;
    using Models;
    using Resources;
    using System.Globalization;

    public class UserService : IUserService
    {
        private readonly IBaseService baseService;
        private readonly IOptionsSnapshot<AppSettingsOptions> configuration;
        private readonly IBaseMailService emailService;

        public UserService(
            IBaseMailService emailService,
            IOptionsSnapshot<AppSettingsOptions> configuration,
            IBaseService baseService)
        {
            this.emailService = emailService;
            this.configuration = configuration;
            this.baseService = baseService;
        }

        public string GenerateTokenOTPRegister(string username)
        {
            string tokenOTPRegister = Utilities.GenerateRadomString();

            var criteria = new UpdateTokenOTPRegisterCriteria
            {
                Username = username,
                TokenOTPRegister = tokenOTPRegister
            };

            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return result.IsSuccessful ? tokenOTPRegister : null;
        }

        public User GetUserByUsername(string username)
        {
            var criteria = new GetUserByUsernameCriteria
            {
                Username = username
            };

            var user = baseService.GetDataEntity<User>(criteria);

            return user;
        }

        public bool IsCorrectPassword(string password, string hashedPassword) => BCrypt.Verify(password, hashedPassword);

        public bool ResetTokenOTPRegister(string username)
        {
            var criteria = new UpdateTokenOTPRegisterCriteria
            {
                Username = username,
                TokenOTPRegister = null
            };

            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return result.IsSuccessful;
        }

        public void ResetUserPassword(string username)
        {
            string tokenResetPassword = Utilities.GenerateRadomString();
            var criteria = new UpdateTokenResetPasswordCriteria
            {
                Username = username,
                TokenResetPassword = tokenResetPassword
            };

            CommandResult updateToken = baseService.ExecuteNonQuery(criteria);

            if (updateToken.IsSuccessful)
            {
                SendMailToResetPassword(username, tokenResetPassword);
            }
        }

        public void SendMailToRegisterOTP(string tokenOTPRegister)
        {
            var user = baseService.GetDataEntity<User>(new GetUserByTokenOTPRegisterCriteria
            {
                TokenOTPRegister = tokenOTPRegister
            });

            var body = emailService.ReadTextFile(Constants.TemplateEmailOTPRegistraion);
            var linkToRegisterOTP = string.Format(CultureInfo.InvariantCulture, "{0}/Account/SetupOTP?token={1}", configuration.Value.BaseHostUrl, tokenOTPRegister);
            body = string.Format(CultureInfo.InvariantCulture, body, user.Username, linkToRegisterOTP);

            var mailModel = new MailModel
            {
                Subject = Message.RegisterOTP,
                ReceiverEmail = user.Email,
                MailCC = string.Empty,
                Body = body
            };

            emailService.SendEmail(mailModel);
        }

        public bool UpdateOTPStatus(int userId, bool isOTPRegistered)
        {
            var criteria = new UpdateUserOTPStatusCriteria
            {
                UserId = userId,
                IsOTPRegistered = isOTPRegistered
            };

            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return result.IsSuccessful;
        }

        public bool UpdateSecretKey(string username, string secretKey)
        {
            var criteria = new UpdateSecretKeyCriteria
            {
                Username = username,
                SecretKey = secretKey
            };

            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return result.IsSuccessful;
        }

        private void SendMailToResetPassword(string username, string tokenResetPassword)
        {
            var user = GetUserByUsername(username);
            var body = emailService.ReadTextFile(Constants.TemplateEmailResetPassword);
            var linkToReset = string.Format(CultureInfo.InvariantCulture, "{0}/Auth/ResetPassword?token={1}", configuration.Value.BaseHostUrl, tokenResetPassword);
            body = string.Format(CultureInfo.InvariantCulture, body, username, linkToReset);

            var mailModel = new MailModel
            {
                Subject = Message.ResetPasswordEmailSubject,
                ReceiverEmail = user.Email,
                MailCC = string.Empty,
                Body = body
            };

            emailService.SendEmail(mailModel);
        }
    }
}