namespace HRP.Models.DataConfiguration.City
{
    using System.Collections.Generic;
    using HRP.Models;

    public class CityTableViewModel
    {
        public IEnumerable<City> Cities { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }
    }
}