namespace HRP.Models
{
    public class Division
    {
        public int DivisionId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int CompanyId { get; set; }

        public bool IsActive { get; set; }
    }
}