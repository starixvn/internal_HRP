using System.ComponentModel.DataAnnotations;

namespace HRP.Core.Enumerations
{
    public enum BankType
    {
        [Display(Name = "e-Banking" )]
        Ebanking = 1,

        [Display(Name = "Direct Banking")]
        DirectBanking = 2,

        [Display(Name = "Contra Co. Account")]
        ContraCoAccount = 3,
    }
}
