namespace HRP.Models.DataConfiguration
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class SimpleItem
    {
        public int ItemId { get; set; }

        [Required]
        [StringLength(50)]
        public string ItemName { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DescriptionMaxLength")]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}