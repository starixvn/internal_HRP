namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;

    public class Paymaster
    {
        public int EmployeeId { get; set; }

        public int Type { get; set; }

        [StringLength(70)]
        public string FullName { get; set; }

        [StringLength(30)]
        public string NickName { get; set; }
    }
}