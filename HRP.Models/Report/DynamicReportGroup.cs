namespace HRP.Models.Report
{
    using System.Collections.Generic;

    public class DynamicReportGroup
    {
        public int GroupdId { get; set;}

        public string Name { get; set; }

        public IEnumerable<DynamicReportItem> Items { get; set; }
    }
}
