namespace HRP.Models.Bank
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateNewBankAccountViewModel
    {
        public CreateNewBankAccountViewModel(string employeeName, IEnumerable<ReferenceResourceModel> bankTypes, IEnumerable<ReferenceResourceModel> bankAccountTypes)
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

        public MultiSelectList BankTypes { get; set; }

        public SelectList BankAccountTypes { get; set; }

        public IBankAccount BankAccount { get; set; }

        public void Initialize(
            string employeeName,
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            EmployeeName = employeeName;

            BankTypes = new MultiSelectList(
                bankTypes,
                nameof(ReferenceResourceModel.ResourceId),
                nameof(ReferenceResourceModel.Value),
                bankTypes.Where(x => x.ResourceId == 1));

            BankAccountTypes = new SelectList(
                bankAccountTypes,
                nameof(ReferenceResourceModel.ResourceId),
                nameof(ReferenceResourceModel.Value),
                bankAccountTypes.Where(x => x.ResourceId == 1));
        }
    }
}