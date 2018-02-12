using System.ComponentModel.DataAnnotations;

namespace HRP.Core.Enumerations
{
    public enum BankTypes
    {
        [Display(Name = "e-Banking" )]
        Ebanking = 1,

        [Display(Name = "Direct Banking")]
        DirectBanking = 2,

        // TODO: Will create bug when user create new Bank Type on DB - Noted by Hunter
        [Display(Name = "Contra Co. Account")]
        ContraCoAccount = 3,
    }
}
