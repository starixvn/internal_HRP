namespace HRP.Models.Report
{
    using System.Collections.Generic;

    public class ColumnGroup
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public IList<MappingColumn> Columns { get; set; }
    }
}