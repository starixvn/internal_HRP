namespace HRP.Models.DataConfiguration.CostCenter
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateCostCenterViewModel
    {
        public SelectList CompanyList { get; set; }

        public CostCenter CostCenter { get; set; } = new CostCenter();

        public Field Field { get; set; }
    }
}