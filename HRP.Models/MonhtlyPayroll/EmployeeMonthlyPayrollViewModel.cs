namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class EmployeeMonthlyPayrollViewModel
    {
        private readonly JsonSerializerSettings jsonSerializerSettings =
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public DateTime Month { get; set; }

        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public int? EmployeeId { get; set; }

        public string CostCenterIds { get; set; }

        public int? WorkingDays { get; set; }

        public IEnumerable<EmployeeMonthlyPayrollBasicInfo> Employees { get; set; } = new List<EmployeeMonthlyPayrollBasicInfo>();

        public IEnumerable<MultiSelectViewModel> HierarchyCompanies { get; set; } = new List<MultiSelectViewModel>();

        public string HierarchyCompanyJson => HierarchyCompanies.Any() ? JsonConvert.SerializeObject(HierarchyCompanies, jsonSerializerSettings) : string.Empty;
    }
}