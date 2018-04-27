namespace HRP.Models.Employee
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class EPA
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameAdministratorRequired))]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameAdministratorMaxLength))]
        public string FullNameAdministrator { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameAdministratorMaxLength))]
        public string NicknameAdministrator { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameEvaluator1MaxLength))]
        public string FullNameEvaluator1 { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameEvaluator1MaxLength))]
        public string NicknameEvaluator1 { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameEvaluator2MaxLength))]
        public string FullNameEvaluator2 { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameEvaluator2MaxLength))]
        public string NicknameEvaluator2 { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameReviewerMaxLength))]
        public string FullNameReviewer { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameReviewerMaxLength))]
        public string NicknameReviewer { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameManagementMaxLength))]
        public string FullNameManagement { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameManagementMaxLength))]
        public string NicknameManagement { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameIncrementMaxLength))]
        public string FullNameIncrement { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameIncrementMaxLength))]
        public string NicknameIncrement { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameBVIBonusMaxLength))]
        public string FullNameBVIBonus { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameBVIBonusMaxLength))]
        public string NicknameBVIBonus { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameLocalBonusMaxLength))]
        public string FullNameLocalBonus { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameLocalBonusMaxLength))]
        public string NicknameLocalBonus { get; set; }
    }
}