namespace HRP.Models.Report
{
    public class DynamicReportColumnModel
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public string HeaderName { get; set; }

        public string ColumnName { get; set; }

        public bool IsEncrypted { get; set; }
    }
}
