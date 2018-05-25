namespace HRP.Services
{
    using System.Globalization;
    using System.Threading.Tasks;
    using BCrypt.Net;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Microsoft.Extensions.Options;
    using Models;
    using Resources;

    public class UserService : IUserService
    {
        private const string EmailTemplateResetPassword = @"wwwroot\\EmailTemplates\\ResetPassword.cshtml";
        private const string EmailTemplateOTPRegistration = @"wwwroot\\EmailTemplates\\OTPRegistration.cshtml";

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

        public async Task<string> GenerateTokenOTPRegisterAsync(string username)
        {
            string tokenOTPRegister = Utilities.GenerateRadomString();

            var criteria = new UpdateTokenOTPRegisterCriteria
            {
                Username = username,
                TokenOTPRegister = tokenOTPRegister
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return result.IsSuccessful ? tokenOTPRegister : null;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var criteria = new GetUserByUsernameCriteria
            {
                Username = username
            };

            return await baseService.GetDataEntityAsync<User>(criteria).ConfigureAwait(false);
        }

        public async Task<User> GetUserByTokenOTPRegisterAsync(string token)
        {
            var criteria = new GetUserByTokenOTPRegisterCriteria
            {
                TokenOTPRegister = token
            };

            return await baseService.GetDataEntityAsync<User>(criteria).ConfigureAwait(false);
        }

        public bool IsCorrectPassword(string password, string hashedPassword) => BCrypt.Verify(password, hashedPassword);

        public async Task<bool> ResetTokenOTPRegisterAsync(string username)
        {
            var criteria = new UpdateTokenOTPRegisterCriteria
            {
                Username = username,
                TokenOTPRegister = null
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return result.IsSuccessful;
        }

        public async Task ResetUserPasswordAsync(string username)
        {
            string tokenResetPassword = Utilities.GenerateRadomString();
            var criteria = new UpdateTokenResetPasswordCriteria
            {
                Username = username,
                TokenResetPassword = tokenResetPassword
            };

            CommandResult updateToken = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            if (updateToken.IsSuccessful)
            {
                await SendMailToResetPasswordAsync(username, tokenResetPassword).ConfigureAwait(false);
            }
        }

        public async Task SendMailToRegisterOTPAsync(string tokenOTPRegister)
        {
            var user = await baseService.GetDataEntityAsync<User>(new GetUserByTokenOTPRegisterCriteria
            {
                TokenOTPRegister = tokenOTPRegister
            }).ConfigureAwait(false);

            var body = emailService.ReadTextFile(EmailTemplateOTPRegistration);
            var linkToRegisterOTP = string.Format(CultureInfo.InvariantCulture, "{0}/Account/SetupOTP?token={1}", configuration.Value.Domain, tokenOTPRegister);
            body = string.Format(CultureInfo.InvariantCulture, body, user.Username, linkToRegisterOTP);

            var mailModel = new MailModel
            {
                Subject = Message.RegisterOTP,
                ReceiverEmail = user.Email,
                Body = body
            };

            emailService.SendEmail(mailModel);
        }

        public async Task<bool> UpdateOTPStatusAsync(int userId, bool isOTPRegistered)
        {
            var criteria = new UpdateUserOTPStatusCriteria
            {
                UserId = userId,
                IsOTPRegistered = isOTPRegistered
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return result.IsSuccessful;
        }

        public async Task<bool> UpdateSecretKeyAsync(string username, string secretKey)
        {
            var criteria = new UpdateSecretKeyCriteria
            {
                Username = username,
                SecretKey = secretKey
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return result.IsSuccessful;
        }

        private async Task SendMailToResetPasswordAsync(string username, string tokenResetPassword)
        {
            var user = await GetUserByUsernameAsync(username).ConfigureAwait(false);
            var body = emailService.ReadTextFile(EmailTemplateResetPassword);
            var linkToReset = string.Format(CultureInfo.InvariantCulture, "{0}/Auth/ResetPassword?token={1}", configuration.Value.Domain, tokenResetPassword);
            body = string.Format(CultureInfo.InvariantCulture, body, username, linkToReset);

            var mailModel = new MailModel
            {
                Subject = Message.ResetPasswordEmailSubject,
                ReceiverEmail = user.Email,
                Body = body
            };

            emailService.SendEmail(mailModel);
        }
    }
}