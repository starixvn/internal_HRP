namespace HRP.Models.Report
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class MonthlyPayrollReportViewModel : BaseSecurityReportViewModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime SelectedTime { get; set; }

        [Display(Name = nameof(ViewLang.Company), ResourceType = typeof(ViewLang))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public int? CompanyId { get; set; }

        [Display(Name = nameof(ViewLang.Currency), ResourceType = typeof(ViewLang))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public int? CurrencyId { get; set; }

        public SelectList Companies { get; set; }
        public SelectList Currencies { get; set; }

        public string CommonError { get; set; }
    }
}