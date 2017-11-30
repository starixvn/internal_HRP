namespace HRP.Core.Extensions
{
    using NPOI.SS.Util;

    public static class CellRangeAddressExtensions
    {
        public static CellRangeAddress Clone(this CellRangeAddressBase range)
        {
            var result = new CellRangeAddress(range.FirstRow, range.LastRow, range.FirstColumn, range.LastColumn);

            return result;
        }
    }
}