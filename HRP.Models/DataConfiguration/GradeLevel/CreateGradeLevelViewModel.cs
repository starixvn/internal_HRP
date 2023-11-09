namespace HRP.Models.DataConfiguration
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateGradeLevelViewModel
    {
        public SelectList JobCategoryList { get; set; }

        public GradeLevel GradeLevel { get; set; } = new GradeLevel();

        public Field Field { get; set; }
    }
}