namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class EPA
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameAdministratorRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameAdministratorMaxLength")]
        public string FullNameAdministrator { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameAdministratorMaxLength")]
        public string NickNameAdministrator { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameEvaluator1MaxLength")]
        public string FullNameEvaluator1 { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameEvaluator1MaxLength")]
        public string NickNameEvaluator1 { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameEvaluator2MaxLength")]
        public string FullNameEvaluator2 { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameEvaluator2MaxLength")]
        public string NickNameEvaluator2 { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameReviewerMaxLength")]
        public string FullNameReviewer { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameReviewerMaxLength")]
        public string NickNameReviewer { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameManagementMaxLength")]
        public string FullNameManagement { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameManagementMaxLength")]
        public string NickNameManagement { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameIncrementMaxLength")]
        public string FullNameIncrement { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameIncrementMaxLength")]
        public string NickNameIncrement { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameBVIBonusMaxLength")]
        public string FullNameBVIBonus { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameBVIBonusMaxLength")]
        public string NickNameBVIBonus { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameLocalBonusMaxLength")]
        public string FullNameLocalBonus { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameLocalBonusMaxLength")]
        public string NickNameLocalBonus { get; set; }
    }
}