namespace HRP.Models.EmployeeManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class EmployeeManagementViewModel
    {
        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public bool IsHostCreateEmployee { get; set; }

        public string DepartmentIds { get; set; }

        public string EmploymentStatusIds { get; set; }

        public int? EmployeeId { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<EmployeeBasicInfo> Employees { get; set; } = new List<EmployeeBasicInfo>();

        public IEnumerable<MultiSelectViewModel> HierarchyCompanies { get; set; } = new List<MultiSelectViewModel>();

        public IEnumerable<MultiSelectViewModel> EmploymentStatuses { get; set; } = new List<MultiSelectViewModel>();

        public string HierarchyCompanyJson => HierarchyCompanies.Any() ? JsonConvert.SerializeObject(HierarchyCompanies, jsonSerializerSettings) : string.Empty;

        public string EmploymentStatusJson => EmploymentStatuses.Any() ? JsonConvert.SerializeObject(EmploymentStatuses, jsonSerializerSettings) : string.Empty;
    }
}