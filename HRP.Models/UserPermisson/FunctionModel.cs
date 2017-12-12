namespace HRP.Models.UserPermisson
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class FunctionModel
    {
        public FunctionModel()
        {
            Childs = new List<FunctionModel>();
        }

        public int FunctionId { get; set; }

        public string FunctionKey { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsActive { get; set; }

        public bool IsAdmin { get; set; }

        public int Level { get; set; }

        public IList<FunctionModel> Childs { get; set; }
    }
}