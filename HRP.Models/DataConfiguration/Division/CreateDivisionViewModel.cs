namespace HRP.Models.DataConfiguration.Division
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateDivisionViewModel
    {
        public CreateDivisionViewModel()
        {
            Division = new Division();
        }

        public SelectList CompanyList { get; set; }

        public Division Division { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }
    }
}