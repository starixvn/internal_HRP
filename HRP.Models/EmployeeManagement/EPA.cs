namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class EPA
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameAdministrator { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameAdministrator { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameEvaluator1 { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameEvaluator1 { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameEvaluator2 { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameEvaluator2 { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameReviewer { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameReviewer { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameManagement { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameManagement { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameIncrement { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameIncrement { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameBVIBonus { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameBVIBonus { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNameLocalBonus { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNameLocalBonus { get; set; }
    }
}