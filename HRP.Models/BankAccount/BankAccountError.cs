namespace HRP.Models.BankAccount
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public enum BankAccountError
    {
        [Display(ResourceType = typeof(Message), Name = "AccountNameExisted")]
        AccountNameExisted = 1,
        [Display(ResourceType = typeof(Message), Name = "AccountNumberExisted")]
        AccountNumberExisted = 2,
        [Display(ResourceType = typeof(Message), Name = "MaximumAccountPerTypeReached")]
        MaximumAccountPerTypeReached = 3
    }
}