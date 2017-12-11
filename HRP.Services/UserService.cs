namespace HRP.Services
{
    using Core.Constants;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Criteria.User;
    using Microsoft.Extensions.Options;
    using Models;
    using Resources;

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
            var criteria = new UpdateTokenResetPasswordCriteria
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

            string linkToReset = _configuration.Value.BaseHostUrl + "/Auth/ResetPassword?token=" + tokenResetPassword;

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

        public string GenerateTokenOTPRegister(string username)
        {
            string tokenOTPRegister = Utilities.GenerateRadomString();

            var criteria = new UpdateTokenOTPRegisterCriteria
            {
                Username = username,
                TokenOTPRegister = tokenOTPRegister
            };

            CommandResult result = _baseService.ExecuteNonQuery(criteria);

            return result.IsSuccessful ? tokenOTPRegister : null;
        }

        public void SendMailToRegisterOTP(string username, string tokenOTPRegister)
        {
            var user = GetUserByUsername(username);

            var body = _emailService.ReadTextFile(Constants.TemplateEmailOTPRegistraion);
            
            string linkToRegisterOTP =
                $"{_configuration.Value.BaseHostUrl}/Account/SetupOTP?token={tokenOTPRegister}&username={username}";

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

        public bool UpdateOTPStatus(int userId, bool isOTPRegistered)
        {
            var criteria = new UpdateUserOTPStatusCriteria
            {
                UserId = userId,
                IsOTPRegistered = isOTPRegistered
            };

            CommandResult result = _baseService.ExecuteNonQuery(criteria);

            return result.IsSuccessful;
        }

        public bool UpdateSecretKey(string username, string secretKey)
        {
            var criteria = new UpdateSecretKeyCriteria
            {
                Username = username,
                SecretKey = secretKey
            };

            CommandResult result = _baseService.ExecuteNonQuery(criteria);

            return result.IsSuccessful;
        }
    }
}