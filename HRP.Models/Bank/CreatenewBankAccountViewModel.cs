namespace HRP.Models.Bank
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateNewBankAccountViewModel
    {
        public CreateNewBankAccountViewModel(
            string employeeName,
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            BankAccount = new BankAccount();
            Initialize(employeeName, bankTypes, bankAccountTypes);
        }

        public CreateNewBankAccountViewModel()
        {
            BankAccount = new BankAccount();
        }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public SelectList BankTypes { get; set; }

        public SelectList BankAccountTypes { get; set; }

        public IBankAccount BankAccount { get; set; }

        public void Initialize(
            string employeeName,
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            EmployeeName = employeeName;

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
}