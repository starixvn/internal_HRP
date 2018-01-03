namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;

    public class EPA
    {
        public int EmployeeId { get; set; }

        public int ProcessId { get; set; }

        public int Priority { get; set; }

        [StringLength(70)]
        public string FullName { get; set; }

        [StringLength(30)]
        public string NickName { get; set; }
    }
}