namespace HRP.Models.BankAccount
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.CustomValidations;
    using HRP.Core.Model;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Resources;

    public class BaseFormBankAccountViewModel
    {
        private const int AccountNumberLength = 30;
        private const int BankAddressLength = 500;
        private const int MediumLength = 70;
        private const int BankSwiftCodeLength = 50;

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.PaymentTransaction))]
        [Required(ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public int PaymentTransactionId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.AccountNumber))]
        [Required(
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(
            AccountNumberLength,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string AccountNumber { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.TypeOfAccount))]
        public int AccountTypeId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.BankAddress))]
        [RequiredIfRelatedFieldIsTrue(
            RelatedFieldName = nameof(BankTypeId),
            DesiredValue = (int)Core.Enumerations.BankTypes.Ebanking,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(BankAddressLength, ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string BankAddress { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.BankBranch))]
        [RequiredIfRelatedFieldIsTrue(
            RelatedFieldName = nameof(BankTypeId),
            DesiredValue = (int)Core.Enumerations.BankTypes.Ebanking,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string BankBranch { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.BankName))]
        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        [Required(
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string BankName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.SwiftCode))]
        [RequiredIfRelatedFieldIsTrue(
            RelatedFieldName = nameof(BankTypeId),
            DesiredValue = (int)Core.Enumerations.BankTypes.Ebanking,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(
            BankSwiftCodeLength,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string BankSwiftCode { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.BankType))]
        [Required(
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public int BankTypeId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.BeneficiaryName))]
        [Required(
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(
            MediumLength,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string BeneficiaryName { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.Active))]
        public bool IsActive { get; set; }

        public bool IsSuccessful { get; set; }

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