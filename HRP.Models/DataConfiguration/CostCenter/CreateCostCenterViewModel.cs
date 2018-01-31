namespace HRP.Models.DataConfiguration.CostCenter
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateCostCenterViewModel
    {
        public CreateCostCenterViewModel()
        {
            CostCenter = new CostCenter();
        }

        public SelectList CompanyList { get; set; }

        public CostCenter CostCenter { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }
    }
}