namespace HRP.Models
{
    public class Company
    {
        public int CompanyId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public bool IsActive { get; set; }
    }
}