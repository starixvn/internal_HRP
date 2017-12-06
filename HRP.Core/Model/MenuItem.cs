namespace HRP.Core.Model
{
    public class MenuItem
    {
        public int MenuId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public int? ParentId { get; set; }

        public int? FunctionId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}