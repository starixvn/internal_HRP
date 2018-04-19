namespace HRP.Core.Model
{
    using System;

    [Serializable]
    public class ReferenceResourceModel
    {
        public int? ResourceId { get; set; }

        public string RefName { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}