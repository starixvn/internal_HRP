namespace HRP.Models.PaymentProcess
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class UploadFilesViewModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime SelectedTime { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public IFormFile AttendanceFile { get; set; }
        public string AttendanceFileName { get; set; }
        public FileResultViewModel AttendanceResult { get; set; }

        public IFormFile DSTNetSalaryFile { get; set; }
        public string DSTNetSalaryFileName { get; set; }
        public FileResultViewModel DSTNetSalaryResult { get; set; }

        public IFormFile AdditionDeductionFile { get; set; }
        public string AdditionDeductionFileName { get; set; }
        public FileResultViewModel AdditionDeductionResult { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DueDate { get; set; }

        public string CommonError { get; set; }
    }
}