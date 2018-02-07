namespace HRP.Models.DataConfiguration.Office
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateOfficeViewModel
    {
        public CreateOfficeViewModel()
        {
            Office = new Office();
        }

        public SelectList CountryList { get; set; }

        public Office Office { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }
    }
}