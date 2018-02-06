namespace HRP.Models.Bank
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeBankAccountsViewModel : EmployeeBankAccounts
    {
        public EmployeeBankAccountsViewModel(
            IEnumerable<BankType> bankTypes,
            IEnumerable<BankAccountType> bankAccountTypes)
        {
            Initialize(bankTypes, bankAccountTypes);
        }

        public EmployeeBankAccountsViewModel()
        {
        }

        public int SelectedBankType { get; set; }

        public SelectList BankTypes { get; set; }

        public bool? IsActiveBankAccount { get; set; }

        public List<SelectListItem> BankAccountActive { get; set; }

        public IEnumerable<BankAccountType> BankAccountTypes { get; set; }

        public void Initialize(IEnumerable<BankType> bankTypes, IEnumerable<BankAccountType> bankAccountTypes)
        {
            BankAccountTypes = bankAccountTypes;

            PopulateBankTypes(bankTypes);
            PopulateActiveOptions();
        }

        private void PopulateActiveOptions()
        {
            BankAccountActive = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "All",
                    Selected = true
                },
                new SelectListItem
                {
                    Value = true.ToString(),
                    Text = "Yes"
                },
                new SelectListItem
                {
                    Value = false.ToString(),
                    Text = "No"
                }
            };
        }

        private void PopulateBankTypes(IEnumerable<BankType> bankTypes)
        {
            var bankTypesWithAllOption = bankTypes.ToList();

            var allOption = new BankType(0, "All");

            bankTypesWithAllOption.Insert(0, allOption);

            BankTypes = new SelectList(
                bankTypesWithAllOption,
                nameof(BankType.Id),
                nameof(BankType.Name));
        }

        public bool CanCreate { get; set; }

        public bool CanEdit { get; set; }
    }
}
