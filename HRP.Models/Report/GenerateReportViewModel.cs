namespace HRP.Models.Report
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class GenerateReportViewModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime SelectedTime { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public SelectList Companies { get; set; }

        [Required]
        public string SelectedIds { get; set; }

        public string EmployeeStatusIds { get; set; }

        public string EmployeeStatuses { get; set; }

        public IEnumerable<IGrouping<string, DynamicReportItem>> Items { get; set; }
    }
}