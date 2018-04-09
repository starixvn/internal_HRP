namespace HRP.Core.Extensions.Excel
{
    using Microsoft.AspNetCore.Http;
    using NPOI.SS.UserModel;

    public static class HSSFWorkbookExtensions
    {
        public static ISheet CloneSheet(this IWorkbook workbook, string sourceSheetName, string newSheetName)
        {
            var sheetIndex = workbook.GetSheetIndex(sourceSheetName);
            var newSheet = workbook.CloneSheet(sheetIndex);
            var newSheetIndex = workbook.GetSheetIndex(newSheet);
            workbook.SetSheetName(newSheetIndex, newSheetName);

            return newSheet;
        }

        public static void ToReponseStream(this IWorkbook workbook, string fileName, HttpResponse response)
        {
            if (workbook != null)
            {
                ////var response = HttpContext.Current.Response;

                response.Clear();
                response.ContentType = "application/vnd.ms-excel";
                response.Headers["Content-Disposition"] = $"attachment; filename={fileName}.xlsx";

                workbook.Write(response.Body);
            }
        }
    }
}