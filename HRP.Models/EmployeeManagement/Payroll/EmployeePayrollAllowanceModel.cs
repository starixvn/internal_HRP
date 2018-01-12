namespace HRP.Models.EmployeeManagement
{
    public class EmployeePayrollAllowanceModel
    {
        public int EmployeeId { get; set; }

        public string CurrencyCode { get; set; }

        public string DirectBanking1CurrencyCode { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public string DirectBanking2CurrencyCode { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public string CashCurrencyCode { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }
    }
}