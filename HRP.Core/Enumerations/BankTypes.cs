namespace HRP.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;

    public enum BankTypes
    {
        [Display(Name = "e-Banking")]
        Ebanking = 1,

        [Display(Name = "Direct Banking")]
        DirectBanking,

        // TODO: Will create bug when user create new Bank Type on DB - Noted by Hunter
        [Display(Name = "Contra Co. Account")]
        ContraCoAccount
    }
}