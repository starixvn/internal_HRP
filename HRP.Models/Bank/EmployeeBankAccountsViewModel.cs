namespace HRP.Models.Bank
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeBankAccountsViewModel
    {
        public EmployeeBankAccountsViewModel()
        {
            BankTypeResources = new List<ReferenceResourceModel>();
            BankAccounts = new List<BankAccount>();
        }

        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        public string EmployeeName { get; set; }

        public IList<BankAccount> BankAccounts { get; set; }

        public int BankTypeId { get; set; }

        public SelectList BankTypes { get {

                var bankTypesWithAllOption = BankTypeResources.ToList();

                var allOption = new ReferenceResourceModel
                {
                    ResourceId = 0,
                    Value = "All"
                };

                bankTypesWithAllOption.Insert(0, allOption);

                var bankTypes = new SelectList(
                    bankTypesWithAllOption,
                    nameof(ReferenceResourceModel.ResourceId),
                    nameof(ReferenceResourceModel.Value));

                return bankTypes;
            } }

        public IEnumerable<ReferenceResourceModel> BankTypeResources { get; set; }

        public bool? IsActive { get; set; }

        public bool HaveHostPermission { get; set; }
    }
}
