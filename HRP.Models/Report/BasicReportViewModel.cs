namespace HRP.Models.Report
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Resources;

    public class BasicReportViewModel
    {
        [Display(Name = nameof(ViewLang.Company), ResourceType = typeof(ViewLang))]
        public int CompanyId { get; set; }

        public SelectList Companies { get; set; }

        [Display(Name = nameof(ViewLang.Month), ResourceType = typeof(ViewLang))]
        public DateTime Month { get; set; }
    }
}