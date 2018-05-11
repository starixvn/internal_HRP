using System.Collections.Generic;

namespace HRP.Models.Report
{
    public class DynamicReportItem
    {
        public DynamicReportItem()
        {
        }

        public DynamicReportItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
