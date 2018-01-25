namespace HRP.Models.Bank
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeBankAccountsViewModel
    {
        public EmployeeBankAccountsViewModel(
            string employeeName,
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            Initialize(employeeName, bankTypes, bankAccountTypes);
        }

        public EmployeeBankAccountsViewModel()
        {
        }

        public int Id { get; set; }

        public string EmployeeName { get; set; }

        public int[] SelectedBankTypes { get; set; }

        public MultiSelectList BankTypes { get; set; }

        public bool? IsActiveBankAccount { get; set; }

        public List<SelectListItem> BankAccountActive { get; set; }

        public IEnumerable<ReferenceResourceModel> BankAccountTypes { get; set; }

        public IList<BankAccount> BankAccounts { get; set; }

        public void Initialize(string employeeName, IEnumerable<ReferenceResourceModel> bankTypes, IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            EmployeeName = employeeName;

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

        private void PopulateBankTypes(IEnumerable<ReferenceResourceModel> bankTypes)
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
                nameof(ReferenceResourceModel.ResourceId),
                nameof(ReferenceResourceModel.Value));
            
            ////BankTypes = bankTypes.ToList();
        }
    }
}
