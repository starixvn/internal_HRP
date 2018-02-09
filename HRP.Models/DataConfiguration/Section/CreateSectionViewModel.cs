namespace HRP.Models.DataConfiguration.Section
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateSectionViewModel
    {
        public CreateSectionViewModel()
        {
            Section = new Section();
        }

        public SelectList CompanyList { get; set; }

        public Section Section { get; set; }

        public Field Field { get; set; }
    }
}