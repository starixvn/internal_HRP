namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class CostCenterTableViewModel
    {
        public IEnumerable<CostCenter> CostCenters { get; set; }

        public Field Field { get; set; }

        public bool IsHost { get; set; }
    }
}