namespace HRP.Services
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Models.Report;
    using NPOI.SS.UserModel;

    public interface IReportService
    {
        IWorkbook Workbook { get; set; }

        ISheet Sheet { get; set; }

        int TitleRowIndex { get; set; }

        int HeaderRowIndex { get; set; }

        int ContentRowStartIndex { get; set; }

        int TotalColumns { get; }

        string FileName { get; set; }

        string SheetName { get; set; }

        string Title { get; set; }

        ICellStyle TitleStyle { get; set; }

        IList<ICellStyle> HeaderStyles { get; set; }

        IList<ICellStyle> ColumnsStyles { get; set; }

        /// <summary>
        /// Hold the condition to apply a style to a cell
        /// </summary>
        IList<ConditionalCellStyle> ConditionalCellStyles { get; set; }

        void Initialize();

        void InitializeWithTemplate(string templatePath);

        /// <summary>
        /// Return index of newly created style in ColumnsStyles list
        /// </summary>
        /// <returns></returns>
        int CreateNewColumnStyle();

        /// <summary>
        /// Export Excel file
        /// </summary>
        /// <param name="datas">data to export</param>
        /// <param name="response">current HttpContext.Response</param>
        /// <param name="firstColumnAutoIncrement">set to true to set first column is row number with auto increment</param>
        void Export(IEnumerable<IExcelRecord> datas, HttpResponse response, bool firstColumnAutoIncrement = false);

        /// <summary>
        /// Export Excel file using Template
        /// </summary>
        /// <param name="datas">data to export</param>
        /// <param name="response">current HttpContext.Response</param>
        /// /// <param name="firstColumnAutoIncrement">set to true to set first column is row number with auto increment</param>
        /// <param name="rewriteTitle">Rewrite title with your text</param>
        void ExportWithTemplate(IEnumerable<IExcelRecord> datas, HttpResponse response, bool firstColumnAutoIncrement = false, bool rewriteTitle = false);
    }
}
