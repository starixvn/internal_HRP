namespace HRP.Models.Report
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public static class ColumnsConfiguration
    {
        private static Dictionary<int, IEnumerable<ColumnGroup>> columns;
        private static IEnumerable<DynamicReportGroup> dynamicReportGroups;

        private static IEnumerable<MappingColumn> GetMappingColumns(IEnumerable<XElement> elements)
        {
            var mappingColumns = new List<MappingColumn>();

            foreach (var element in elements)
            {
                mappingColumns.Add(new MappingColumn
                {
                    PropertyName = element.Attribute("PropertyName").Value,
                    HeaderName = element.Attribute("HeaderName").Value,
                    IsEncrypted = bool.Parse(element.Attribute("IsEncrypted").Value)
                });
            }

            return mappingColumns;
        }

        private static IEnumerable<ColumnGroup> GetColumnGroups(IEnumerable<XElement> elements)
        {
            var columnGroups = new List<ColumnGroup>();
            foreach (var element in elements)

            {
                columnGroups.Add(new ColumnGroup
                {
                    GroupId = int.Parse(element.Attribute("GroupId").Value),
                    GroupName = element.Attribute("GroupName").Value,
                    Columns = GetMappingColumns(element.Elements())
                });
            }

            return columnGroups;
        }

        private static IEnumerable<DynamicReportItem> GetItems(IEnumerable<XElement> elements)
        {
            var items = new List<DynamicReportItem>();

            foreach (var element in elements)
            {
                var id = int.Parse(element.Attribute("Id").Value);
                columns.Add(id, GetColumnGroups(element.Elements("ColumnGroup")));

                items.Add(new DynamicReportItem
                {
                    Id = id,
                    Name = element.Attribute("Name").Value,
                });
            }

            return items;
        }

        private static void InitializeConfiguration()
        {
            var groups = new List<DynamicReportGroup>();
            columns = new Dictionary<int, IEnumerable<ColumnGroup>>();
            var xml = XElement.Load(@"App_Data\ColumnsConfiguration.xml");

            foreach (var groupElement in xml.Elements("Group"))
            {
                var dynamicReportGroup = new DynamicReportGroup
                {
                    GroupdId = int.Parse(groupElement.Attribute("GroupdId").Value),
                    Name = groupElement.Attribute("Name").Value,
                    Items = GetItems(groupElement.Elements("Item"))
                };

                groups.Add(dynamicReportGroup);
            }

            dynamicReportGroups = groups;
        }

        public static Dictionary<int, IEnumerable<ColumnGroup>> Columns
        {
            get
            {
                if (columns == null)
                {
                    InitializeConfiguration();
                }

                return columns;
            }
        }

        public static IEnumerable<DynamicReportGroup> DynamicReportGroups
        {
            get
            {
                if (dynamicReportGroups == null)
                {
                    InitializeConfiguration();
                }

                return dynamicReportGroups;
            }
        }
    }
}