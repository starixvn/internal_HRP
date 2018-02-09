namespace HRP.Models.DataConfiguration.GradeLevel
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateGradeLevelViewModel
    {
        public CreateGradeLevelViewModel()
        {
            GradeLevel = new GradeLevel();
        }

        public SelectList JobCategoryList { get; set; }

        public GradeLevel GradeLevel { get; set; }

        public Field Field { get; set; }
    }
}