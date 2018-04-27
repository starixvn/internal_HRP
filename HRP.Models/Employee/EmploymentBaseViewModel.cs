namespace HRP.Models.Employee
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmploymentBaseViewModel : Employment
    {
        public SelectList Companies { get; set; }

        public SelectList LocalCompanies { get; set; }

        public SelectList LocalJobTitles { get; set; }

        public SelectList EmploymentTypes { get; set; }

        public SelectList EmploymentStatuses { get; set; }

        public SelectList JobCategories { get; set; }

        public SelectList Countries { get; set; }

        public SelectList WorkPermitType1s { get; set; }

        public SelectList WorkPermitType2s { get; set; }
    }
}