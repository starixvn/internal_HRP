namespace HRP.Models.Bank
{
    public class BankType
    {
        public BankType(ReferenceResourceModel referenceResource)
        {
            Id = referenceResource.ResourceId;
            Name = referenceResource.Value;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
