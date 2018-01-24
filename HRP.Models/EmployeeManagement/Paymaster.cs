namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class Paymaster
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVISalaryRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVISalaryMaxLength")]
        public string FullNameBVISalary { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameBVISalaryMaxLength")]
        public string NicknameBVISalary { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVIAllowanceRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVIAllowanceMaxLength")]
        public string FullNameBVIAllowance { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameBVIAllowanceMaxLength")]
        public string NicknameBVIAllowance { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameLocalSalaryMaxLength")]
        public string FullNameLocalSalary { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameLocalSalaryMaxLength")]
        public string NicknameLocalSalary { get; set; }
    }
}