namespace HRP.Models
{
    using System;

    [Serializable]
    public class Country
    {
        public int CountryId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}