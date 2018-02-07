namespace HRP.Models.DataConfiguration.Office
{
    using System.Collections.Generic;
    using HRP.Models;

    public class OfficeTableViewModel
    {
        public IEnumerable<Office> Offices { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }
    }
}