namespace HRP.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public enum BankTypes
    {
        [Display(Name = nameof(ViewLang.EBanking), ResourceType = typeof(ViewLang))]
        Ebanking = 1,

        [Display(Name = nameof(ViewLang.DirectBanking), ResourceType = typeof(ViewLang))]
        DirectBanking,

        [Display(Name = nameof(ViewLang.Cash), ResourceType = typeof(ViewLang))]
        Cash
    }
}