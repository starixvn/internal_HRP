namespace HRP.Models.DataConfiguration.CostCenter
{
    using System.Collections.Generic;
    using HRP.Models;

    public class CostCenterTableViewModel
    {
        public IEnumerable<CostCenter> CostCenters { get; set; }

        public Field Field { get; set; }
    }
}