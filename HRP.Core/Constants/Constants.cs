namespace HRP.Core.Constants
{
    public static class Constants
    {
        public const string DateFormat = "dd MMM, yyyy";

        public const string TimeFormat = "HH:mm";

        public const string DateTimeFormat = "dd MMM, yyyy HH:mm";

        public const string BeginRootTag = "<ROOT>";

        public const string EndRootTag = "</ROOT>";

        public const string TemplateEmailResetPassword = "wwwroot/EmailTemplates/EmailTemplateResetPassword.cshtml";

        public const string PasswordRequiredMessage = "Password must contain at least 8 characters, including UPPER/lowercase and number.";

        public const string PasswordNotMatch = "Password doesn't match, try again";
    }
}