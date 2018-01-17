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
    }
}