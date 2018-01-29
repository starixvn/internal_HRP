namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class DivisionTableViewModel
    {
        public IEnumerable<DivisionRow> Divisions { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }
    }
}