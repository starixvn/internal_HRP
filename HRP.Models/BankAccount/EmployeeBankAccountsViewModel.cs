namespace HRP.Models.BankAccount
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Resources;

    public class EmployeeBankAccountsViewModel
    {
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        public string EmployeeName { get; set; }

        public IList<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

        public int? BankTypeId { get; set; }

        public SelectList BankTypes
        {
            get
            {
                var bankTypesWithAllOption = BankTypeResources.ToList();
                var allOption = new ReferenceResourceModel
                {
                    Value = ViewLang.All
                };

                bankTypesWithAllOption.Insert(0, allOption);

                return new SelectList(
                    bankTypesWithAllOption,
                    nameof(ReferenceResourceModel.ResourceId),
                    nameof(ReferenceResourceModel.Value));
            }
        }

        public IEnumerable<ReferenceResourceModel> BankTypeResources { get; set; } = new List<ReferenceResourceModel>();

        public bool? IsActive { get; set; }

        public bool HaveHostPermission { get; set; }

        ////public bool BankAccountsLimitReached { get; set; }
    }
}