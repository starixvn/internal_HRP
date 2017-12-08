namespace HRP.Services
{
    using Core.Model;
    using Core.Services;
    using HRP.Core.Constants;
    using HRP.Criteria;
    using HRP.Criteria.User;
    using HRP.Models;
    using HRP.Resources;
    using Microsoft.Extensions.Options;

    public class UserService : IUserService
    {
        private readonly IOptionsSnapshot<AppSettingsOptions> _configuration;
        private readonly IBaseMailService _emailService;
        private readonly IBaseService _baseService;

        public UserService() : this(null, null, null)
        {
        }

        public UserService(IBaseMailService emailService,
                IOptionsSnapshot<AppSettingsOptions> configuration,
                IBaseService baseService)
        {
            _emailService = emailService;
            _configuration = configuration;
            _baseService = baseService;
        }

        public User GetUserByUsername(string username)
        {
            var criteria = new GetUserByUsernameCriteria
            {
                Username = username
            };

            var user = _baseService.GetDataEntity<User>(criteria);

            return user;
        }

        public void ResetUserPassword(string username)
        {
            string tokenResetPassword = Utilities.GenerateRadomString();
            var criteria = new UpdateTokenResetPasswordCriteria()
            {
                Username = username,
                TokenResetPassword = tokenResetPassword
            };

            CommandResult updateToken = _baseService.ExecuteNonQuery(criteria);

            if (updateToken.IsSuccessful)
            {
                SendMailToResetPassword(username, tokenResetPassword);
            }
        }

        private void SendMailToResetPassword(string username, string tokenResetPassword)
        {
            var user = GetUserByUsername(username);

            var body = _emailService.ReadTextFile(Constants.TemplateEmailResetPassword);

            string linkToReset = _configuration.Value.BaseHostUrl + "/ResetPassword?token=" + tokenResetPassword;

            body = string.Format(body, username, linkToReset);

            var mailModel = new MailModel
            {
                Subject = Message.ResetPasswordEmailSubject,
                ReceiverEmail = user.Email,
                MailCC = string.Empty,
                Body = body
            };

            _emailService.SendEmail(mailModel);
        }

        public void OTPRegister(string username)
        {
            string tokenOTPRegister = Utilities.GenerateRadomString();
            var criteria = new UpdateTokenOTPRegisterCriteria()
            {
                Username = username,
                TokenRegisterOTP = tokenOTPRegister
            };

            CommandResult result = _baseService.ExecuteNonQuery(criteria);

            if (result.IsSuccessful)
            {
                SendMailToRegisterOTP(username, tokenOTPRegister);
            }
        }

        private void SendMailToRegisterOTP(string username, string tokenOTPRegister)
        {
            var user = GetUserByUsername(username);

            var body = _emailService.ReadTextFile(Constants.TemplateEmailOTPRegistraion);

            string linkToRegisterOTP = _configuration.Value.BaseHostUrl + "/OTPRegister?token=" + tokenOTPRegister;

            body = string.Format(body, username, linkToRegisterOTP);

            var mailModel = new MailModel
            {
                Subject = Message.RegisterOTP,
                ReceiverEmail = user.Email,
                MailCC = string.Empty,
                Body = body
            };

            _emailService.SendEmail(mailModel);
        }
    }
}