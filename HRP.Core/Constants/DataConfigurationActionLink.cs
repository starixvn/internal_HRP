using System;

namespace HRP.Core.Constants
{
    public static class DataConfigurationActionLink
    {
        public const string DataConfiguration = "DataConfiguration";

        public const string CreateCompanyLocal = DataConfiguration + "/CreateCompanyLocal";
        public const string UpdateCompanyLocal = DataConfiguration + "/UpdateCompanyLocal";
        public const string GetAllCompanyLocal = DataConfiguration + "/GetAllCompanyLocal";

        public const string CreateCompanyBVI = DataConfiguration + "/CreateCompanyBVI";
        public const string UpdateCompanyBVI = DataConfiguration + "/UpdateCompanyBVI";
        public const string GetAllCompanyBVI = DataConfiguration + "/GetAllCompanyBVI";

        public const string CreateDivision = DataConfiguration + "/CreateDivision";
        public const string UpdateDivision = DataConfiguration + "/UpdateDivision";
        public const string GetAllDivision = DataConfiguration + "/GetAllDivision";
    }
}