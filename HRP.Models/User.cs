namespace HRP.Models
{
    using System;

    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool IsTwoFactor { get; set; }

        public string SecretKey { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}