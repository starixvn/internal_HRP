namespace HRP.Models
{
    using System;

    [Serializable]
    public class Division
    {
        public int DivisionId { get; set; }

        public int CompanyId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}