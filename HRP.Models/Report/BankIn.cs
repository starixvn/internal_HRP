namespace HRP.Models.Report
{
    public class BankIn : IExcelRecord
    {
        public string Nickname { get; set; }

        public string BeneficiaryName { get; set; }

        public string BankName { get; set; }

        public string AccountNumber { get; set; }

        public string Salary { get; set; }

        public string EbankingAmount { get; set; }

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
