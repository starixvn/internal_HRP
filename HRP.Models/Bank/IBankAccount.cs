namespace HRP.Models.Bank
{
    using System.ComponentModel.DataAnnotations;
    ////using Microsoft.AspNetCore.Mvc;
    using Resources;

    // For StringLengthAttribute source code:
    // https://github.com/Microsoft/referencesource/blob/master/System.ComponentModel.DataAnnotations/DataAnnotations/StringLengthAttribute.cs
    // Line 76

    public interface IBankAccount
    {
        int Id { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "AccountName")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        ////[Remote(
        ////    action: "CheckAccountNameExisted",
        ////    controller: "BankAccount",
        ////    AdditionalFields = nameof(Id) + "," + nameof(EmployeeId) + "," + nameof(BankType))]
        //// Turn off client validation for instantly check Account Name
        string AccountName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "AccountNumber")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        ////[Remote(
        ////    action: "CheckAccountNumberExisted",
        ////    controller: "BankAccount",
        ////    AdditionalFields = nameof(Id) + "," + nameof(EmployeeId) + "," + nameof(BankType))]
        //// Turn off client validation for instantly check Account Number
        string AccountNumber { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "TypeOfAccount")]
        int AccountType { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BankAddress")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        string BankAddress { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BankBranch")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        string BankBranch { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BankName")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        string BankName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "SwiftCode")]
        ////[Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        string BankSwiftCode { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BankType")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        int BankType { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BeneficiaryName")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        string BeneficiaryName { get; set; }

        int EmployeeId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "Active")]
        bool IsActive { get; set; }
    }
}