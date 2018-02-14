namespace HRP.Models.DataConfiguration.City
{
    using System.Collections.Generic;
    using HRP.Models;

    public class CityTableViewModel
    {
        public IEnumerable<City> Cities { get; set; }

        public Field Field { get; set; }

        public bool IsHost { get; set; }
    }
}