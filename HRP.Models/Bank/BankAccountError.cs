namespace HRP.Models.Bank
{
    using System.ComponentModel.DataAnnotations;

    public enum BankAccountError
    {
        [Display(Name = "Account Name existed. Please enter another value")]
        AccountNameExisted = 1,
        [Display(Name = "Account Number existed. Please enter another value")]
        AccountNumberExisted = 2,
        [Display(Name = "This employee has reached the limit number of active bank accounts in selected type")]
        MaximumAccountPerTypeReached = 3
    }
}