namespace HRP.Models.Report
{
    using System;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class BasicReportViewModel
    {
        public int CompanyId { get; set; }

        public SelectList Companies { get; set; }

        public DateTime Month { get; set; }
    }
}