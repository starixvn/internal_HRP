namespace HRP.Models.Report
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class GenerateReportViewModel
    {
        [Required]
        public int CompanyId { get; set; }

        public SelectList Companies { get; set; }

        public string SelectedIds { get; set; }

        public string EmployeeStatusIds { get; set; }

        public string EmployeeStatuses { get; set; }

        [RequiredIfRelatedFieldIsTrue(DesiredValue = false, RelatedFieldName = nameof(ReusePassword),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordRequired))]
        public string Password { get; set; }

        public bool ReusePassword { get; set; }

        public IEnumerable<DynamicReportGroup> Groups { get; set; }
    }
}