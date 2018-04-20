namespace HRP.Models.Report
{
    using Core.Extensions.Excel;

    public class BankIn
    {
        [EpplusIgnore]
        public string Nickname { get; set; }

        public string BeneficiaryName { get; set; }

        public string BankName { get; set; }

        public string AccountNumber { get; set; }

        public string PaymentTransaction { get; set; }

        [EpplusIgnore]
        public string Amount { get; set; }

        public float AmountDecrypted { get; set; }

        [EpplusIgnore]
        public string CurrencyCode { get; set; }
    }
}
