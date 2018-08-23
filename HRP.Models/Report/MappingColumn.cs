namespace HRP.Models.Report
{
    public class MappingColumn
    {
        public string PropertyName { get; set; }

        public string HeaderName { get; set; }

        public bool IsEncrypted { get; set; }

        public bool IsAmount { get; set; }
    }
}