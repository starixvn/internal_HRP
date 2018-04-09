namespace HRP.Services
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core.Extensions.Excel;
    using Microsoft.AspNetCore.Http;
    using Models.Report;

    [ExcludeFromCodeCoverage]
    public class ReportService : IReportService
    {
        public ReportService()
        {
        }

        public ReportService(string fileName, string sheetName, string title) : this()
        {
            FileName = fileName;
            SheetName = sheetName;
            Title = title;
        }

        public IWorkbook Workbook { get; set; }

        public ISheet Sheet { get; set; }

        public int TitleRowIndex { get; set; }

        public int HeaderRowIndex { get; set; } = 1;

        public int ContentRowStartIndex { get; set; } = 2;

        public int TotalColumns { get; private set; }

        public string FileName { get; set; }

        public string SheetName { get; set; }

        public string Title { get; set; }

        public ICellStyle TitleStyle { get; set; }

        public IList<ICellStyle> HeaderStyles { get; set; }

        public IList<ICellStyle> ColumnsStyles { get; set; }

        public IList<ConditionalCellStyle> ConditionalCellStyles { get; set; }

        public void Initialize()
        {
            Workbook = new HSSFWorkbook();
            Sheet = Workbook.CreateSheet(SheetName);

            TitleStyle = Workbook.CreateCellStyle();
            HeaderStyles = new List<ICellStyle>();

            ConditionalCellStyles = new List<ConditionalCellStyle>();
            ColumnsStyles = new List<ICellStyle>();
        }

        public void InitializeWithTemplate(string templatePath)
        {
            try
            {
                using (FileStream fireStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    Workbook = new HSSFWorkbook(fireStream, true);
                    Sheet = Workbook.GetSheetAt(0);

                    ConditionalCellStyles = new List<ConditionalCellStyle>();
                    ColumnsStyles = new List<ICellStyle>();
                }
            }
            catch (IOException exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        private void WriteTitle()
        {
            IRow titleRow = Sheet.CreateRow(TitleRowIndex);
            ICell cell = titleRow.CreateCell(0, CellType.String);
            cell.SetCellValue(Title);
            cell.CellStyle = TitleStyle;
        }

        private void WriteHeaders(object sampleRecord)
        {
            Type recordType = sampleRecord.GetType();

            var columns = recordType.GetProperties();

            IRow headerRow = Sheet.CreateRow(HeaderRowIndex);

            for (var index = 0; index < TotalColumns; index++)
            {
                PropertyInfo propertyInfo = columns[index];
                headerRow.CreateCell(index).SetCellValue(propertyInfo.Name);
                HeaderStyles.Add(Workbook.CreateCellStyle());
            }
        }

        private void WriteData(IEnumerable<IExcelRecord> datas, bool firstColumnAutoIncrement)
        {
            int rowId = ContentRowStartIndex;

            int rowNumber = 1;

            foreach (IExcelRecord data in datas)
            {
                IRow row = Sheet.CreateRow(rowId);
                WriteEachColumn(firstColumnAutoIncrement, rowNumber, data, row);

                rowId++;
                rowNumber++;
            }
        }

        private void WriteEachColumn(bool firstColumnAutoIncrement, int rowNumber, IExcelRecord data, IRow row)
        {
            var objectData = data.ConvertToExcelRecord();
            PropertyInfo[] infos = objectData.GetType().GetProperties();

            for (int index = 0; index < TotalColumns; index++)
            {
                ICell cell = row.CreateCell(index);

                var value = infos[index].GetValue(objectData, null);

                cell.SetCellValue(value);

                if (index == 0 && firstColumnAutoIncrement)
                {
                    cell.SetCellValue(rowNumber);
                }

                foreach (ConditionalCellStyle conditionalCellStyle in ConditionalCellStyles)
                {
                    if (index == conditionalCellStyle.ColumnIndex
                        && conditionalCellStyle.Condition(objectData))
                    {
                        cell.CellStyle = ColumnsStyles[conditionalCellStyle.StyleIndex];
                    }
                }
            }
        }

        public int CreateNewColumnStyle()
        {
            if (ColumnsStyles != null)
            {
                ICellStyle style = Workbook.CreateCellStyle();
                ColumnsStyles.Add(style);
                return ColumnsStyles.IndexOf(style);
            }

            return 0;
        }

        private void FormatSheet()
        {
            for (int i = 0; i < TotalColumns; i++)
            {
                Sheet.AutoSizeColumn(i);
            }

            Sheet.CreateFreezePane(0, ContentRowStartIndex);
        }

        private void CopyTitleStyleFromTemplate()
        {
            TitleStyle = Sheet.GetRow(TitleRowIndex).GetCell(0).CellStyle;
        }

        public void Export(IEnumerable<IExcelRecord> datas, HttpResponse response, bool firstColumnAutoIncrement = false)
        {
            var dataList = datas as IList<IExcelRecord> ?? datas.ToList();

            if (!dataList.Any())
            {
                return;
            }

            TotalColumns = dataList.First().ConvertToExcelRecord().GetType().GetProperties().Length;

            WriteTitle();

            WriteHeaders(dataList.First().ConvertToExcelRecord());

            WriteData(dataList, firstColumnAutoIncrement);

            FormatSheet();

            Workbook.ToReponseStream(FileName, response);
        }

        public void ExportWithTemplate(
            IEnumerable<IExcelRecord> datas,
            HttpResponse response,
            bool firstColumnAutoIncrement = false,
            bool rewriteTitle = false)
        {
            ////var dataList = datas as IList<IExcelRecord> ?? datas.ToList();

            if (datas == null || !datas.Any())
            {
                return;
            }

            TotalColumns = datas.First().ConvertToExcelRecord().GetType().GetProperties().Length;

            if (rewriteTitle)
            {
                CopyTitleStyleFromTemplate();
                WriteTitle();
            }

            WriteData(datas, firstColumnAutoIncrement);

            FormatSheet();

            Workbook.ToReponseStream(FileName, response);
        }
    }
}
