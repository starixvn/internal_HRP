namespace HRP.Models.Report
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class GenerateReportViewModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime SelectedTime { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public SelectList Companies { get; set; }

        public string CommonError { get; set; }

        public bool IsHasIDNo { get; set; }
        public bool IsHasFullName { get; set; }
        public bool IsHasNickName { get; set; }
        public bool IsHasGender { get; set; }
        public bool IsHasDateOfBirth { get; set; }
        public bool IsHasPlaceOfBirth { get; set; }
        public bool IsHasNationality { get; set; }
    }
}