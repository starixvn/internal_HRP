namespace HRP.Models.Report
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Resources;

    public class BankInReportViewModel
    {
        [Display(Name = nameof(ViewLang.Company), ResourceType = typeof(ViewLang))]
        public int CompanyId { get; set; }

        public SelectList Companies { get; set; }

        [Display(Name = nameof(ViewLang.MonthYear), ResourceType = typeof(ViewLang))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime SelectedTime { get; set; }

        [Display(Name = nameof(ViewLang.TypeOfPayment), ResourceType = typeof(ViewLang))]
        public PaymentType TypeOfPayment { get; set; }
    }
}
