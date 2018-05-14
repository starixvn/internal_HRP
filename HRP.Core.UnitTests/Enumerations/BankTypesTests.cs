namespace HRP.Core.UnitTests.Enumerations
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Extensions;
    using Resources;
    using Xunit;

    public class BankTypesTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void BankTypes_GetName_ReturnExpectedDisplayName(BankTypes bankType, string displayName)
        {
            Assert.Equal(bankType.GetDisplayName(), displayName);
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { BankTypes.Ebanking, ViewLang.EBanking },
                new object[] { BankTypes.DirectBanking, ViewLang.DirectBanking },
                new object[] { BankTypes.Cash, ViewLang.Cash }
            };
    }
}
