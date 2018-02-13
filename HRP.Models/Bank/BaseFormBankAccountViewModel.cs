namespace HRP.Models.Bank
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class BaseFormBankAccountViewModel
    {
        public BaseFormBankAccountViewModel()
        {
            IsActive = true;
        }

        public int Id { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "AccountName")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        ////[Remote(
        ////    action: "CheckAccountNameExisted",
        ////    controller: "BankAccount",
        ////    AdditionalFields = nameof(Id) + "," + nameof(EmployeeId) + "," + nameof(BankType))]
        //// Turn off client validation for instantly check Account Name
        public string AccountName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "AccountNumber")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        ////[Remote(
        ////    action: "CheckAccountNumberExisted",
        ////    controller: "BankAccount",
        ////    AdditionalFields = nameof(Id) + "," + nameof(EmployeeId) + "," + nameof(BankType))]
        //// Turn off client validation for instantly check Account Number
        public string AccountNumber { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "TypeOfAccount")]
        public int AccountType { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BankAddress")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        public string BankAddress { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BankBranch")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        public string BankBranch { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BankName")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        public string BankName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "SwiftCode")]
        ////[Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        public string BankSwiftCode { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BankType")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        public int BankType { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BeneficiaryName")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        public string BeneficiaryName { get; set; }

        public int EmployeeId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "Active")]
        public bool IsActive { get; set; }
    }
}