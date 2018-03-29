namespace HRP.Models.Payslip
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class PayslipViewModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime Month { get; set; }

        public int CompanyId { get; set; }
        public bool? IsSent { get; set; }
        public IEnumerable<PayslipModel> Payslips { get; set; }
        public SelectList Companies { get; set; }
    }
}