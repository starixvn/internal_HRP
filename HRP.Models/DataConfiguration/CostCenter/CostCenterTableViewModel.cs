namespace HRP.Models.DataConfiguration.CostCenter
{
    using System.Collections.Generic;
    using HRP.Models;

    public class CostCenterTableViewModel
    {
        public IEnumerable<CostCenter> CostCenters { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }
    }
}