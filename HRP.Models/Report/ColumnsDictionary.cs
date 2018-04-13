namespace HRP.Models.Report
{
    using System.Collections.Generic;

    public static class ColumnsDictionary
    {
        private static readonly Dictionary<int, ColumnGroup> columns = new Dictionary<int, ColumnGroup>
        {
            {1,  new ColumnGroup{
                GroupId = 1,
                GroupName = "Full Name",
                Columns = new List<MappingColumn> {
                   new MappingColumn{
                       PropertyName = nameof(DynamicReportModel.FullName),
                       HeaderName = "Full Name"
                   }}}
            },
            {2,  new ColumnGroup{
                GroupId = 2,
                GroupName = "Nick Name",
                Columns = new List<MappingColumn> {
                   new MappingColumn{
                       PropertyName = nameof(DynamicReportModel.Nickname),
                       HeaderName = "Nick Name"
                   }}}
            },
            {3,  new ColumnGroup{
                GroupId = 3,
                GroupName = "Gender",
                Columns = new List<MappingColumn> {
                   new MappingColumn{
                       PropertyName = nameof(DynamicReportModel.Gender),
                       HeaderName = "Gender"
                   }}}
            },
            {4,  new ColumnGroup{
                GroupId = 4,
                GroupName = "Date Of Birth",
                Columns = new List<MappingColumn> {
                   new MappingColumn{
                       PropertyName = nameof(DynamicReportModel.DateOfBirth),
                       HeaderName = "Date Of Birth"
                   }}}
            },
            {5,  new ColumnGroup{
                GroupId = 5,
                GroupName = "NRICNo",
                Columns = new List<MappingColumn> {
                   new MappingColumn{
                       PropertyName = nameof(DynamicReportModel.NRICNo),
                       HeaderName = "NRICNo"
                   }}}
            }
        };

        public static Dictionary<int, ColumnGroup> Columns { get => columns; }
    }
}