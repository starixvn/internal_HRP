namespace HRP.Models.RefResource
{
    public class RefResourceContructable : IRefResourceConstructable
    {
        public RefResourceContructable(ReferenceResourceModel referenceResource)
        {
            Id = referenceResource.ResourceId;
            Name = referenceResource.Value;
        }

        public RefResourceContructable(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int? Id { get; set; }
        public string Name { get; set; }
    }
}