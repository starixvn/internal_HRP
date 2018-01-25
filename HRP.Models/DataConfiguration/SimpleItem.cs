namespace HRP.Models.DataConfiguration
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    public class SimpleItem
    {
        public int ItemId { get; set; }

        [Required]
        [StringLength(50)]
        [Remote("")]
        public string ItemName { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DescriptionMaxLength")]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public static string FieldName { get; set; }
    }
}