namespace HRP.Models.PaymentProcess
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class UploadAttendanceAndSalaryViewModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime SelectedTime { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public IFormFile AttendanceFile { get; set; }

        public IFormFile DSTSalaryFile { get; set; }

        public string CommonError { get; set; }
    }
}