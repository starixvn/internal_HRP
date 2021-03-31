namespace HRP.Models.Report
{
    public class BankIn
    {
        public string IDNo { get; set; }

        public string BeneficiaryName { get; set; }

        public string Nickname { get; set; }

        public int PaymentTransactionId { get; set; }
        public string PaymentTransaction { get; set; }

        public string BankName { get; set; }

        public string BankBranch { get; set; }

        public string BankAddress { get; set; }

        public string BankSwiftCode { get; set; }

        public string AccountNumber { get; set; }

        public string Amount { get; set; }

        public float AmountDecrypted { get; set; }

        public string CurrencyCode { get; set; }
    }
}
