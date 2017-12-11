namespace HRP.Models.UserPermisson
{
    using System;
    using System.Xml.Serialization;
    using Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class UserPermissionModel
    {
        [XmlIgnore]
        public int UserId { get; set; }

        public int FunctionId { get; set; }

        public int CompanyId { get; set; }

        public bool IsRead { get; set; }

        public bool IsHost { get; set; }
    }
}