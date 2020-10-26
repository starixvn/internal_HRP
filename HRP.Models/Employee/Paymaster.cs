namespace HRP.Models.Employee
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class Paymaster
    {
        private const int SmallLength = 30;
        private const int MediumLength = 70;

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameBVISalaryRequired))]
        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameBVISalaryMaxLength))]
        public string FullNameBVISalary { get; set; }

        [StringLength(SmallLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameBVISalaryMaxLength))]
        public string NicknameBVISalary { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameBVIAllowanceRequired))]
        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameBVIAllowanceMaxLength))]
        public string FullNameBVIAllowance { get; set; }

        [StringLength(SmallLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameBVIAllowanceMaxLength))]
        public string NicknameBVIAllowance { get; set; }

        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameLocalSalaryMaxLength))]
        public string FullNameLocalSalary { get; set; }

        [StringLength(SmallLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameLocalSalaryMaxLength))]
        public string NicknameLocalSalary { get; set; }
    }
}