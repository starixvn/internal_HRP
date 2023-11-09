namespace HRP.Models.DataConfiguration
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateCostCenterViewModel
    {
        public SelectList CompanyList { get; set; }

        public CostCenter CostCenter { get; set; } = new CostCenter();

        public Field Field { get; set; }
    }
}