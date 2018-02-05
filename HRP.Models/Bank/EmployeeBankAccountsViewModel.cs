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

        public int[] SelectedBankTypes { get; set; }

        public MultiSelectList BankTypes { get; set; }

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
                    Text = "True"
                },
                new SelectListItem
                {
                    Value = false.ToString(),
                    Text = "False"
                }
            };
        }

        private void PopulateBankTypes(IEnumerable<BankType> bankTypes)
        {
            ////var allOption = new ReferenceResourceModel
            ////{
            ////    ResourceId = 0,
            ////    Value = "All"
            ////};

            var bankTypesWithAllOption = bankTypes.ToList();
            ////bankTypesWithAllOption.Add(allOption);

            BankTypes = new MultiSelectList(
                bankTypesWithAllOption,
                nameof(BankType.Id),
                nameof(BankType.Name));

            ////BankTypes = bankTypes.ToList();
        }

        public bool CanCreate { get; set; }

        public bool CanEdit { get; set; }
    }
}
