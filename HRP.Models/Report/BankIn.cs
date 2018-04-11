namespace HRP.Models.Report
{
    using Core.Extensions.Excel;

    public class BankIn : IExcelRecord
    {
        public string Nickname { get; set; }

        public string BeneficiaryName { get; set; }

        public string BankName { get; set; }

        public string AccountNumber { get; set; }

        public string Salary { get; set; }

        [EpplusIgnore]
        public string EbankingAmount { get; set; }

        [EpplusIgnore]
        public string DirectBanking1Amount { get; set; }

        [EpplusIgnore]
        public string DirectBanking2Amount { get; set; }

        [EpplusIgnore]
        public string CashAmount { get; set; }

        [EpplusIgnore]
        public string CompanyName { get; set; }

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Nickname,
                BeneficiaryName,
                BankName,
                AccountNumber,
                Salary,
                EbankingAmount
            };

            return record;
        }
    }
}
