namespace HRP.Models.UserManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Models.Account;
    using HRP.Resources;

    public class EditUserViewModel : UserProfileViewModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "Status")]
        public bool IsActive { get; set; }

        public bool IsSystemAdmin { get; set; }

        public bool CanEdit { get; set; }
    }
}