namespace HRP.Models.PaymentProcess
{
    using System.Xml.Serialization;
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class AdditionDeductionUploading : TemplateUploading
    {
        [XmlIgnore]
        public int EmployeeId { get; set; }

        public string Salary_Addition_Adjustments_Comment { get; set; }
        public string Salary_Addition_Adjustments_Amount { get; set; }
        public string Salary_Addition_Adjustments_NoOfDays { get; set; }

        public string Salary_Addition_Others1_Comment { get; set; }
        public string Salary_Addition_Others1_Amount { get; set; }
        public string Salary_Addition_Others1_NoOfDays { get; set; }
        public string Salary_Addition_Others2_Comment { get; set; }
        public string Salary_Addition_Others2_Amount { get; set; }
        public string Salary_Addition_Others2_NoOfDays { get; set; }

        public string Salary_Deduction_Absenteeism_Comment { get; set; }
        public string Salary_Deduction_Absenteeism_Days { get; set; }
        public string Salary_Deduction_Tardiness_Comment { get; set; }
        public string Salary_Deduction_Tardiness_Minutes { get; set; }
        public string Salary_Deduction_Advance_Comment { get; set; }
        public string Salary_Deduction_Advance_Amount { get; set; }
        public string Salary_Deduction_Others_Comment { get; set; }
        public string Salary_Deduction_Others_Amount { get; set; }

        public string Allowance_Addition_Subsistence_Amount { get; set; }
        public string Allowance_Addition_Subsistence_CurrencyCode { get; set; }
        public string Allowance_Addition_Subsistence_NoOfDays { get; set; }
        public string Allowance_Addition_HomePassage_Amount { get; set; }
        public string Allowance_Addition_HomePassage_CurrencyCode { get; set; }
        public string Allowance_Addition_HomePassage_NoOfDays { get; set; }
        public string Allowance_Addition_Housing_Amount { get; set; }
        public string Allowance_Addition_Housing_CurrencyCode { get; set; }
        public string Allowance_Addition_Housing_NoOfDays { get; set; }
        public string Allowance_Addition_MobilePhone_Amount { get; set; }
        public string Allowance_Addition_MobilePhone_CurrencyCode { get; set; }
        public string Allowance_Addition_MobilePhone_NoOfDays { get; set; }
        public string Allowance_Addition_HPI_Amount { get; set; }
        public string Allowance_Addition_HPI_CurrencyCode { get; set; }
        public string Allowance_Addition_HPI_NoOfDays { get; set; }
        public string Allowance_Addition_DutyMeal_CurrencyCode { get; set; }
        public string Allowance_Addition_DutyMeal_NoOfDays { get; set; }
        public string Allowance_Addition_Secondment_Amount { get; set; }
        public string Allowance_Addition_Secondment_CurrencyCode { get; set; }
        public string Allowance_Addition_Secondment_NoOfDays { get; set; }
        public string Allowance_Addition_SpecialAllowance_Amount { get; set; }
        public string Allowance_Addition_SpecialAllowance_CurrencyCode { get; set; }
        public string Allowance_Addition_SpecialAllowance_NoOfDays { get; set; }
        public string Allowance_Addition_OT_Amount { get; set; }
        public string Allowance_Addition_OT_CurrencyCode { get; set; }
        public string Allowance_Addition_OT_NoOfDays { get; set; }
        public string Allowance_Addition_Others_Amount { get; set; }
        public string Allowance_Addition_Others_CurrencyCode { get; set; }
        public string Allowance_Addition_Others_NoOfDays { get; set; }
        public string Allowance_Addition_Comment { get; set; }

        public string Allowance_Deduction_Subsistence_Amount { get; set; }
        public string Allowance_Deduction_Subsistence_CurrencyCode { get; set; }
        public string Allowance_Deduction_Subsistence_NoOfDays { get; set; }
        public string Allowance_Deduction_HomePassage_Amount { get; set; }
        public string Allowance_Deduction_HomePassage_CurrencyCode { get; set; }
        public string Allowance_Deduction_HomePassage_NoOfDays { get; set; }
        public string Allowance_Deduction_Housing_Amount { get; set; }
        public string Allowance_Deduction_Housing_CurrencyCode { get; set; }
        public string Allowance_Deduction_Housing_NoOfDays { get; set; }
        public string Allowance_Deduction_MobilePhone_Amount { get; set; }
        public string Allowance_Deduction_MobilePhone_CurrencyCode { get; set; }
        public string Allowance_Deduction_MobilePhone_NoOfDays { get; set; }
        public string Allowance_Deduction_HPI_Amount { get; set; }
        public string Allowance_Deduction_HPI_CurrencyCode { get; set; }
        public string Allowance_Deduction_HPI_NoOfDays { get; set; }
        public string Allowance_Deduction_DutyMeal_CurrencyCode { get; set; }
        public string Allowance_Deduction_DutyMeal_NoOfDays { get; set; }
        public string Allowance_Deduction_Secondment_Amount { get; set; }
        public string Allowance_Deduction_Secondment_CurrencyCode { get; set; }
        public string Allowance_Deduction_Secondment_NoOfDays { get; set; }
        public string Allowance_Deduction_Others_Amount { get; set; }
        public string Allowance_Deduction_Others_CurrencyCode { get; set; }
        public string Allowance_Deduction_Others_NoOfDays { get; set; }
        public string Allowance_Deduction_Comment { get; set; }
    }
}