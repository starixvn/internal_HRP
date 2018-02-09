namespace HRP.Models.DataConfiguration.Office
{
    using System.Collections.Generic;
    using HRP.Models;

    public class OfficeTableViewModel
    {
        public IEnumerable<Office> Offices { get; set; }

        public Field Field { get; set; }
    }
}