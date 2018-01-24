namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class EPA
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameAdministratorRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameAdministratorMaxLength")]
        public string FullNameAdministrator { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameAdministratorMaxLength")]
        public string NicknameAdministrator { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameEvaluator1MaxLength")]
        public string FullNameEvaluator1 { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameEvaluator1MaxLength")]
        public string NicknameEvaluator1 { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameEvaluator2MaxLength")]
        public string FullNameEvaluator2 { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameEvaluator2MaxLength")]
        public string NicknameEvaluator2 { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameReviewerMaxLength")]
        public string FullNameReviewer { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameReviewerMaxLength")]
        public string NicknameReviewer { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameManagementMaxLength")]
        public string FullNameManagement { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameManagementMaxLength")]
        public string NicknameManagement { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameIncrementMaxLength")]
        public string FullNameIncrement { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameIncrementMaxLength")]
        public string NicknameIncrement { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVIBonusMaxLength")]
        public string FullNameBVIBonus { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameBVIBonusMaxLength")]
        public string NicknameBVIBonus { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameLocalBonusMaxLength")]
        public string FullNameLocalBonus { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameLocalBonusMaxLength")]
        public string NicknameLocalBonus { get; set; }
    }
}