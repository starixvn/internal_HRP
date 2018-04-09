namespace HRP.Models.Report
{
    public class BasicSalaryDirectBankingAndCashModel
    {
        public int EmployeeId { get; set; }

        public int PayrollId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string CostCenterCode { get; set; }

        public string AccountName1 { get; set; }

        public string BankName1 { get; set; }

        public string AccountNumber1 { get; set; }

        public string AccountName2 { get; set; }

        public string BankName2 { get; set; }

        public string AccountNumber2 { get; set; }

        public string DirectBanking1Currency { get; set; }

        public string DirectBanking1Amount { get; set; }

        public string DirectBanking2Currency { get; set; }

        public string DirectBanking2Amount { get; set; }

        public string CashCurrency { get; set; }

        public string CashAmount { get; set; }
    }
}