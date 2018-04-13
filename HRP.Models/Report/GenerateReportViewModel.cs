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

        public int[] ColumnGroupIds { get; set; }

        public SelectList ColumnGroups { get; set; }

        public string CommonError { get; set; }

        [Display(Name = "ID No")]
        public bool IsHasIDNo { get; set; }

        [Display(Name = "FullName")]
        public bool IsHasFullName { get; set; }

        [Display(Name = "NickName")]
        public bool IsHasNickName { get; set; }

        public bool IsHasGender { get; set; }
        public bool IsHasDateOfBirth { get; set; }
        public bool IsHasPlaceOfBirth { get; set; }
        public bool IsHasNationality { get; set; }
    }
}