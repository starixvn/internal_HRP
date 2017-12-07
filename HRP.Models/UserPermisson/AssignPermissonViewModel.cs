namespace HRP.Models.UserPermisson
{
    using System.Collections.Generic;

    public class AssignPermissonViewModel
    {
        public int UserId { get; set; }

        public IEnumerable<FunctionModel> Functions { get; set; }

        public IEnumerable<Company> Companies { get; set; }

        public IEnumerable<UserPermissionModel> UserPermissions { get; set; }

        public bool IsHost { get; set; }
    }
}