namespace HRP.Models
{
    using System;

    [Serializable]
    public class CostCenter
    {
        public int CostCenterId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentCode { get; set; }

        public int DivisionId { get; set; }

        public string DivisionCode { get; set; }

        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }
    }
}