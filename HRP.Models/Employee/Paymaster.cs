namespace HRP.Models.Employee
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class Paymaster
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameBVISalaryRequired))]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameBVISalaryMaxLength))]
        public string FullNameBVISalary { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameBVISalaryMaxLength))]
        public string NicknameBVISalary { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameBVIAllowanceRequired))]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameBVIAllowanceMaxLength))]
        public string FullNameBVIAllowance { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameBVIAllowanceMaxLength))]
        public string NicknameBVIAllowance { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameLocalSalaryMaxLength))]
        public string FullNameLocalSalary { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameLocalSalaryMaxLength))]
        public string NicknameLocalSalary { get; set; }
    }
}