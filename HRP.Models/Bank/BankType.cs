namespace HRP.Models.Bank
{
    public class BankType : RefResourceContructable
    {
        public BankType(ReferenceResourceModel referenceResource) : base(referenceResource)
        {
        }

        public BankType(int id, string name) : base(id, name)
        {
        }
    }
}