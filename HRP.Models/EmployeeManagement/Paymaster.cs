namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class Paymaster
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVISalaryRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVISalaryMaxLength")]
        public string FullNameBVISalary { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameBVISalaryMaxLength")]
        public string NickNameBVISalary { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVIAllowanceRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVIAllowanceMaxLength")]
        public string FullNameBVIAllowance { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameBVIAllowanceMaxLength")]
        public string NickNameBVIAllowance { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameLocalSalaryMaxLength")]
        public string FullNameLocalSalary { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameLocalSalaryMaxLength")]
        public string NickNameLocalSalary { get; set; }
    }
}