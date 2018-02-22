namespace HRP.Models.BankAccount
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Resources;

    public class BaseFormBankAccountViewModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "AccountName")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        public string AccountName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "AccountNumber")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        public string AccountNumber { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "TypeOfAccount")]
        public int AccountTypeId { get; set; }

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
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        public string BankSwiftCode { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BankType")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        public int BankTypeId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "BeneficiaryName")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StringLengthMaxErrorMessage")]
        public string BeneficiaryName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "Active")]
        public bool IsActive { get; set; }

        public SelectList BankTypes => new SelectList(
            BankTypeResources,
            nameof(ReferenceResourceModel.ResourceId),
            nameof(ReferenceResourceModel.Value));

        public SelectList BankAccountTypes => new SelectList(
            BankAccountTypeResources,
            nameof(ReferenceResourceModel.ResourceId),
            nameof(ReferenceResourceModel.Value));

        public bool HaveHostPermission { get; set; }

        public IEnumerable<ReferenceResourceModel> BankTypeResources { get; set; }

        public IEnumerable<ReferenceResourceModel> BankAccountTypeResources { get; set; }
    }
}