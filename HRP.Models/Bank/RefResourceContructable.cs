namespace HRP.Models.Bank
{
    using System.Collections.Generic;

    public class RefResourceContructable : IRefResourceConstructable
    {
        public RefResourceContructable(ReferenceResourceModel referenceResource)
        {
            Id = referenceResource.ResourceId;
            Name = referenceResource.Value;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface IRefResourceConstructable
    {
        int Id { get; set; }
        string Name { get; set; }
    }

    public class RefResourceContructableComparer : IEqualityComparer<RefResourceContructable>
    {
        public bool Equals(RefResourceContructable x, RefResourceContructable y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            return x.Id == y.Id;

        }

        public int GetHashCode(RefResourceContructable obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}