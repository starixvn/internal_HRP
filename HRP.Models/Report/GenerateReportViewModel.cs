namespace HRP.Models.Report
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class GenerateReportViewModel : BaseSecurityReportViewModel
    {
        [Required]
        [Display(Name = nameof(ViewLang.Company), ResourceType = typeof(ViewLang))]
        public int CompanyId { get; set; }

        public SelectList Companies { get; set; }

        public string SelectedIds { get; set; }

        public string EmployeeStatusIds { get; set; }

        public string EmployeeStatuses { get; set; }

        public IEnumerable<DynamicReportGroup> Groups { get; set; }
    }
}