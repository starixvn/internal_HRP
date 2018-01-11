namespace HRP.Models.Bank
{
    using System.ComponentModel.DataAnnotations;

    public interface IBankAccount
    {
        [Display(Name = "Account Name")]
        string AccountName { get; set; }

        [Display(Name = "Account Number")]
        string AccountNumber { get; set; }

        [Display(Name = "Type of Account")]
        int AccountType { get; set; }

        [Display(Name = "Address")]
        string BankAddress { get; set; }

        [Display(Name = "Branch")]
        string BankBranch { get; set; }

        [Display(Name = "Bank Name")]
        string BankName { get; set; }

        [Display(Name = "SWIFT Code")]
        string BankSwiftCode { get; set; }

        [Display(Name = "Bank Type")]
        int BankType { get; set; }

        [Display(Name = "Beneficiary Name")]
        string BeneficiaryName { get; set; }
        
        int EmployeeId { get; set; }

        [Display(Name = "Active")]
        bool IsActive { get; set; }
    }
}