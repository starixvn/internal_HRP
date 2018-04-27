namespace HRP.Models.Employee
{
    using System.Collections.Generic;
    using HRP.Core.Model;

    public class EmploymentBaseViewModel : Employment
    {
        public IEnumerable<Company> Companies { get; set; } = new List<Company>();

        public IEnumerable<ReferenceResourceModel> LocalCompanies { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> LocalJobTitles { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> EmploymentTypes { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> EmploymentStatuses { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<JobCategory> JobCategories { get; set; } = new List<JobCategory>();

        public IEnumerable<Country> Countries { get; set; } = new List<Country>();

        public IEnumerable<ReferenceResourceModel> WorkPermitType1s { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> WorkPermitType2s { get; set; } = new List<ReferenceResourceModel>();
    }
}