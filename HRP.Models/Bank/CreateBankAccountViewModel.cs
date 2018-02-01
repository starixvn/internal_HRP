namespace HRP.Models.Bank
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateBankAccountViewModel
    {
        public CreateBankAccountViewModel(
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            BankAccount = new BankAccount();
            Initialize(bankTypes, bankAccountTypes);
        }

        public CreateBankAccountViewModel()
        {
            BankAccount = new BankAccount();
        }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public SelectList BankTypes { get; set; }

        public SelectList BankAccountTypes { get; set; }

        public IBankAccount BankAccount { get; set; }

        public bool IsBankAccountExisted { get; set; }

        public BankAccountError Error { get; set; }

        public void Initialize(
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            BankTypes = new SelectList(
                bankTypes,
                nameof(ReferenceResourceModel.ResourceId),
                nameof(ReferenceResourceModel.Value));

            BankAccountTypes = new SelectList(
                bankAccountTypes,
                nameof(ReferenceResourceModel.ResourceId),
                nameof(ReferenceResourceModel.Value));
        }
    }

    public enum BankAccountError
    {
        [Display(Name = "Account Name and Account Number existed. Please enter another value")]
        AccountExisted = 1
    }
}