namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class EmployeeMonthlyPayrollViewModel
    {
        private readonly JsonSerializerSettings jsonSerializerSettings =
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        [DisplayFormat(DataFormatString = "{0:MM-yyyy}")]
        public DateTime Month { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DueDate { get; set; }

        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public int? EmployeeId { get; set; }

        public string CostCenterIds { get; set; }

        public int? WorkingDays { get; set; }

        public IEnumerable<MonthlyPayrollEmployeeInfoViewModel> Employees { get; set; } = new List<MonthlyPayrollEmployeeInfoViewModel>();

        public IEnumerable<MultiSelectViewModel> HierarchyCompanies { get; set; } = new List<MultiSelectViewModel>();

        public string HierarchyCompanyJson => HierarchyCompanies.Any() ? JsonConvert.SerializeObject(HierarchyCompanies, jsonSerializerSettings) : string.Empty;
    }
}