namespace HRP.Models.UserManagement
{
    using System.Collections.Generic;

    public class UserManagementViewModel
    {
        public bool? Status { get; set; }

        public bool IsHost { get; set; }

        public IEnumerable<UserModel> Users { get; set; } = new List<UserModel>();
    }
}