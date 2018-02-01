namespace HRP.Models.DataConfiguration.CostCenter
{
    using HRP.Models;

    public class UpdateCostCenterViewModel
    {
        public string FieldName { get; set; }

        public string LinkUpdateItem { get; set; }

        public CostCenter CostCenter { get; set; }
    }
}