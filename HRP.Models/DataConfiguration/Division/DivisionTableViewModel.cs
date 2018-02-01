namespace HRP.Models.DataConfiguration.Division
{
    using System.Collections.Generic;
    using HRP.Models;

    public class DivisionTableViewModel
    {
        public IEnumerable<Division> Divisions { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }
    }
}