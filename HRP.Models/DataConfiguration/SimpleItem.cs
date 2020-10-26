namespace HRP.Models.DataConfiguration
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class SimpleItem
    {
        private const int ItemNameLength = 50;
        private const int DescriptionLength = 200;

        public int ItemId { get; set; }

        [Required]
        [StringLength(ItemNameLength)]
        public string ItemName { get; set; }

        [StringLength(DescriptionLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DescriptionMaxLength))]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}