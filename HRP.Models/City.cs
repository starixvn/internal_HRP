namespace HRP.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class City
    {
        [Required]
        public int CityId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DescriptionMaxLength")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public int CountryId { get; set; }

        public string CountryCode { get; set; }
    }
}