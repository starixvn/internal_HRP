namespace HRP.Models
{
    public class CompanyTree
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int ParentId { get; set; }

        public int CategoryId { get; set; }
    }
}
