namespace HRP.Models.UnitTests
{
    using System.Collections.Generic;
    using HRP.Models.Report;
    using Xunit;

    public class BankInReportTests
    {
        [Fact]
        public void IsExportable_BankInsIsAllNullOrEmpty_ReturnFalse()
        {
            var model = new BankInReport
            {
                EBankingBankIns = null,
                CashBankIns = null
            };

            var result = model.IsExportable;

            Assert.False(result);
        }

        [Fact]
        public void IsExportable_EBankingBankInsIsNotNull_ReturnTrue()
        {
            var model = new BankInReport
            {
                EBankingBankIns = new List<BankIn>
                {
                    new BankIn()
                },
                CashBankIns = null
            };

            var result = model.IsExportable;

            Assert.True(result);
        }

        [Fact]
        public void IsExportable_CashBankInsIsNotNull_ReturnTrue()
        {
            var model = new BankInReport
            {
                EBankingBankIns = null,
                CashBankIns = new List<BankIn>
                {
                    new BankIn()
                },
            };

            var result = model.IsExportable;

            Assert.True(result);
        }
    }
}
