namespace HRP.Models.DataConfiguration
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateSectionViewModel
    {
        public SelectList CompanyList { get; set; }

        public Section Section { get; set; } = new Section();

        public Field Field { get; set; }
    }
}