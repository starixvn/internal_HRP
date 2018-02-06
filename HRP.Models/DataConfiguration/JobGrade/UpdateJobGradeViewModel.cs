namespace HRP.Models.DataConfiguration.JobGrade
{
    using HRP.Models;

    public class UpdateJobGradeViewModel
    {
        public string FieldName { get; set; }

        public string LinkUpdateItem { get; set; }

        public JobGrade JobGrade { get; set; }
    }
}