namespace HRP.Models.Report
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
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

        public string EmployeeStatuses { get; set; }
    }
}