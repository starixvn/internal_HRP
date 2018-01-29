namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DivisionTableViewModel
    {
        public IEnumerable<DivisionItem> Divisions { get; set; }

        public SelectList CompanyList { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }

        public Division NewDivision { get; set; }
    }
}