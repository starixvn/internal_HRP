namespace HRP.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;

    public enum BankTypes
    {
        [Display(Name = "e-Banking")]
        Ebanking = 1,

        [Display(Name = "Direct Banking")]
        DirectBanking
    }
}