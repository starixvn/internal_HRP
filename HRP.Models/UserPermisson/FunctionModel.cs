namespace HRP.Models.UserPermisson
{
    public class FunctionModel
    {
        public int FunctionId { get; set; }

        public string FunctionKey { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsActive { get; set; }

        public bool IsAdmin { get; set; }
    }
}