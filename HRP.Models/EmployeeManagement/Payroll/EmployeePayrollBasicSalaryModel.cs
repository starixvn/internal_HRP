namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class EmployeePayrollBasicSalaryModel : EmployeePayrollBaseBankDetails
    {
        public int EmployeeId { get; set; }

        public string Salary { get; set; }

        public string OldSalary { get; set; }

        public string CurrencyId { get; set; }

        public string OldCurrencyId { get; set; }

        public int WorkingDays { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(EbankingAmount), nameof(EbankingAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollCurrencyRequried")]
        public string EbankingCurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(EbankingCurrencyId), nameof(EbankingAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollSettingAmountRequried")]
        public string EbankingAmount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(EbankingCurrencyId), nameof(EbankingAmount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollAccountRequried")]
        public int? EbankingAccountId { get; set; }

        public string EbankingAccountName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EffectiveDate { get; set; }

        public int? ReasonId { get; set; }

        public bool InitFirstTime { get; set; }
    }
}