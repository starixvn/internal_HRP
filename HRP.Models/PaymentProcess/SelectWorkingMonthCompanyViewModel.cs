namespace HRP.Models.PaymentProcess
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SelectWorkingMonthCompanyViewModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime SelectedTime { get; set; }

        public int CompanyId { get; set; }

        public SelectList Companies { get; set; }
    }
}
