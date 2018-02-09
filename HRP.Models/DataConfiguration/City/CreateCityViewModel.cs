namespace HRP.Models.DataConfiguration.City
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateCityViewModel
    {
        public CreateCityViewModel()
        {
            City = new City();
        }

        public SelectList CountryList { get; set; }

        public City City { get; set; }

        public Field Field { get; set; }
    }
}