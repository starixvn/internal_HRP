namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class Paymaster
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameBVISalary { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameBVISalary { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameBVIAllowance { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameBVIAllowance { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameLocalSalary { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameLocalSalary { get; set; }
    }
}