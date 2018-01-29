namespace HRP.Models.DataConfiguration
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateDivisionViewModel
    {
        public SelectList CompanyList { get; set; }

        public Division Division { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }
    }
}