namespace HRP.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy hh:mm tt}")]
        public DateTime CreatedDate { get; set; }

        public string Email { get; set; }

        public int CreatedBy { get; set; }

        public bool IsActive { get; set; }
    }
}