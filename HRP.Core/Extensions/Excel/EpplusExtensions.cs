namespace HRP.Core.Extensions.Excel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using OfficeOpenXml;

    public static class EpplusExtensions
    {
        public static ExcelRangeBase LoadFromCollectionFiltered<T>(
            this ExcelRangeBase excelRangeBase,
            IEnumerable<T> collection)
            where T : class
        {
            var result = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => !Attribute.IsDefined(x, typeof(EpplusIgnoreAttribute))).ToArray();

            var membersToInclude = new MemberInfo[result.Length];

            for (int i = 0; i < result.Length; i++)
            {
                membersToInclude[i] = result[i];
            }

            return excelRangeBase.LoadFromCollection(
                collection,
                false,
                OfficeOpenXml.Table.TableStyles.None,
                BindingFlags.Instance | BindingFlags.Public,
                membersToInclude);
        }
    }
}
