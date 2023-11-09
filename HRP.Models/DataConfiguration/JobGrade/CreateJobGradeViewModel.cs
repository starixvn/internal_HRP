namespace HRP.Models.DataConfiguration
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateJobGradeViewModel
    {
        public SelectList JobCategoryList { get; set; }

        public JobGrade JobGrade { get; set; } = new JobGrade();

        public Field Field { get; set; }
    }
}