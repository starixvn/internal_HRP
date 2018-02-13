namespace HRP.Models.Bank
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateBankAccountViewModel
    {
        public CreateBankAccountViewModel()
        {
            BankTypeResources = new List<ReferenceResourceModel>();
            BankAccountTypeResources = new List<ReferenceResourceModel>();
            BankAccount = new BaseFormBankAccountViewModel();
        }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public SelectList BankTypes
        {
            get
            {
                var bankTypes = new SelectList(
                    BankTypeResources,
                    nameof(ReferenceResourceModel.ResourceId),
                    nameof(ReferenceResourceModel.Value));

                return bankTypes;
            }
        }

        public SelectList BankAccountTypes
        {
            get
            {
                var bankTypes = new SelectList(
                    BankAccountTypeResources,
                    nameof(ReferenceResourceModel.ResourceId),
                    nameof(ReferenceResourceModel.Value));

                return bankTypes;
            }
        }

        public BaseFormBankAccountViewModel BankAccount { get; set; }

        public bool HaveHostPermission { get; set; }

        public IEnumerable<ReferenceResourceModel> BankTypeResources { get; set; }

        public IEnumerable<ReferenceResourceModel> BankAccountTypeResources { get; set; }
    }
}