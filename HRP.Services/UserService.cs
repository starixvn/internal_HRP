namespace HRP.Services
{
    using Core.Model;
    using Core.Services;
    using HRP.Core.Constants;
    using HRP.Criteria;
    using HRP.Criteria.User;
    using HRP.Models;
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

        public User GetUserByTokenResetPassword(string tokenResetPassword)
        {
            var criteria = new GetUserByTokenResetPasswordCriteria
            {
                TokenResetPassword = tokenResetPassword
            };

            return _baseService.GetDataEntity<User>(criteria);
        }

        public CommandResult UpdateTokenResetPassword(string username, string token)
        {
            var criteria = new UpdateTokenResetPasswordCriteria
            {
                Username = username,
                TokenResetPassword = token
            };

            var result = _baseService.ExecuteNonQuery(criteria);

            return result;
        }

        public void ResetUserPassword(string username)
        {
            string tokenResetPassword = Utilities.GenerateRadomString();
            CommandResult updateToken = UpdateTokenResetPassword(username, tokenResetPassword);

            if (updateToken.IsSuccessful)
            {
                SendMailToResetPassword(username, tokenResetPassword);
            }
        }

        private void SendMailToResetPassword(string username, string tokenResetPassword)
        {
            var user = GetUserByUsername(username);

            var body = _emailService.ReadTextFile(Constants.TemplateEmailResetPassword);

            string linkToReset = _configuration.Value.BaseHostUrl + "/reset-password?token=" + tokenResetPassword;

            body = string.Format(body, username, linkToReset);

            MailModel model = new MailModel
            {
                Subject = "Reset your password on HRP system",
                ReceiverEmail = user != null ? user.Email : "oliver.dang@nexcel.vn",
                MailCC = string.Empty,
                Body = body
            };

            _emailService.SendEmail(model);
        }
    }
}