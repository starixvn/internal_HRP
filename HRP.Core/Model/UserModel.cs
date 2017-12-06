namespace HRP.Core.Model
{
    using System;

    public class UserModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Phone { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Email { get; set; }

        public int CreatedBy { get; set; }

        public bool IsActive { get; set; }
    }
}