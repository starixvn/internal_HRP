namespace HRP.Models
{
    using System;

    [Serializable]
    public class Office
    {
        public int OfficeId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}