namespace HRP.Core.Extensions
{
    using Microsoft.AspNetCore.Http;
    using NPOI.SS.UserModel;

    public static class WorkbookExtensions
    {
        public static ISheet CloneSheet(this IWorkbook workbook, string sourceSheetName, string newSheetName)
        {
            var sheetIndex = workbook.GetSheetIndex(sourceSheetName);
            var newSheet = workbook.CloneSheet(sheetIndex);
            var newSheetIndex = workbook.GetSheetIndex(newSheet);
            workbook.SetSheetName(newSheetIndex, newSheetName);
            
            return newSheet;
        }
    }
}