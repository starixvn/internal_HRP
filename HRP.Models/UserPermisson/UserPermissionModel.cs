namespace HRP.Models.UserPermisson
{
    public class UserPermissionModel
    {
        public int UserId { get; set; }

        public int FunctionId { get; set; }

        public int CompanyId { get; set; }

        public bool IsRead { get; set; }

        public bool IsHost { get; set; }
    }
}