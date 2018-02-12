namespace HRP.Models.RefResource
{
    using System.Collections.Generic;

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