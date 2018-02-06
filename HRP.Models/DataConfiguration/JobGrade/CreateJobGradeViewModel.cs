namespace HRP.Models.DataConfiguration.JobGrade
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateJobGradeViewModel
    {
        public CreateJobGradeViewModel()
        {
            JobGrade = new JobGrade();
        }

        public SelectList JobCategoryList { get; set; }

        public JobGrade JobGrade { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }
    }
}