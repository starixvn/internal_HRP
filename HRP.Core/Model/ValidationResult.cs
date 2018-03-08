namespace HRP.Core.Model
{
    using System.Xml.Serialization;
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class ValidationResult
    {
        public int RowNumber { get; set; }

        public int ResultType { get; set; }

        public string Message { get; set; }

        [XmlIgnore]
        public bool IsValid => ResultType == 1;
    }
}