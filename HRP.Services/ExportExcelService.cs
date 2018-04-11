namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Core.Extensions.Excel;
    using Interfaces;
    using Models.Report;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;
    using OfficeOpenXml;

    [ExcludeFromCodeCoverage]
    public class ExportExcelService : IExportExcelService
    {
        public Stream GetPayDetails(Stream fileStream)
        {
            var workbook = new XSSFWorkbook(fileStream);
            var templateSheet = workbook.GetSheetAt(0);
            var sheet = workbook.CreateSheet("Pay Details");

            for (var i = 0; i < 9; i++)
            {
                CopyRow(sheet, templateSheet, i, i, 0, int.MaxValue - 1);
            }

            var memory = new MemoryStream();
            sheet.Workbook.RemoveSheetAt(0);
            sheet.CreateFreezePane(4, 9);
            sheet.Workbook.Write(memory);
            memory.Position = 0;

            return memory;
        }

        public static void CopyRow(
            ISheet sheet,
            ISheet sourceSheet,
            int sourceRowIndex,
            int rowIndex,
            int firstColumnIndex,
            int lastColumnIndex)
        {
            // Expand the sheet to satisfy the number of needed rows.
            for (var i = sheet.LastRowNum + 1; i <= rowIndex; i++)
            {
                sheet.CreateRow(i);
            }

            // Get source row that will be copied.
            var sourceRow = sourceSheet.GetRow(sourceRowIndex);
            if (sourceRow != null)
            {
                // Get the row at a specific index of the current sheet.
                var newRow = sheet.GetRow(rowIndex);
                newRow = newRow ?? sheet.CreateRow(rowIndex);
                newRow.Height = sourceRow.Height;

                // Get the first and last available columns of the copying area.
                int firstCellNum = Math.Min(sourceRow.FirstCellNum, firstColumnIndex);
                int lastCellNum = Math.Min(sourceRow.LastCellNum, lastColumnIndex + 1);

                // Loop through each cell of the source row to copy its data to the new row.
                for (int i = firstCellNum; i < lastCellNum; i++)
                {
                    // Get the source cell.
                    var sourceCell = sourceRow.GetCell(i);

                    // If the source cell is null then jump to next cell
                    if (sourceCell == null)
                    {
                        continue;
                    }

                    // Create a new cell at current sheet, and clone the source cell data and style to it.
                    var newCell = newRow.CreateCell(i);
                    CloneCell(newCell, sourceCell);

                    // Copy merged area.
                    CopyMergedArea(sheet, sourceSheet, sourceCell, newCell);
                }
            }
        }

        private static void CloneCell(ICell newCell, ICell sourceCell)
        {
            if (sourceCell != null)
            {
                if (sourceCell.CellComment != null)
                {
                    newCell.CellComment = sourceCell.CellComment;
                }

                if (sourceCell.Hyperlink != null)
                {
                    newCell.Hyperlink = sourceCell.Hyperlink;
                }

                if (sourceCell.CellStyle != null)
                {
                    newCell.CellStyle = sourceCell.CellStyle;
                }

                newCell.SetCellType(sourceCell.CellType);

                switch (sourceCell.CellType)
                {
                    case CellType.Boolean:
                        {
                            newCell.SetCellValue(sourceCell.BooleanCellValue);
                            break;
                        }

                    case CellType.Numeric:
                        {
                            newCell.SetCellValue(sourceCell.NumericCellValue);
                            break;
                        }

                    case CellType.String:
                        {
                            newCell.SetCellValue(sourceCell.StringCellValue);
                            break;
                        }

                    case CellType.Error:
                        {
                            newCell.SetCellValue(sourceCell.ErrorCellValue);
                            break;
                        }

                    case CellType.Formula:
                        {
                            newCell.CellFormula = sourceCell.CellFormula;
                            break;
                        }

                    default:
                        newCell.SetCellValue(string.Empty);
                        break;
                }
            }
        }

        private static void CopyMergedArea(ISheet sheet, ISheet sourceSheet, ICell sourceCell, ICell newCell)
        {
            CellRangeAddress sourceRange = null;
            int j = 0;

            while (sourceRange == null && j < sourceSheet.NumMergedRegions)
            {
                CellRangeAddress range = sourceSheet.GetMergedRegion(j);

                if (range.FirstColumn == sourceCell.ColumnIndex && range.FirstRow == sourceCell.RowIndex)
                {
                    sourceRange = range;
                }

                j += 1;
            }

            if (sourceRange != null)
            {
                var newRange = new CellRangeAddress(
                    newCell.RowIndex,
                    newCell.RowIndex + sourceRange.LastRow - sourceRange.FirstRow,
                    newCell.ColumnIndex,
                    newCell.ColumnIndex + sourceRange.LastColumn - sourceRange.FirstColumn);

                sheet.AddMergedRegion(newRange);
            }
        }

        public byte[] ExportBankInReports(
            DateTime selectedTime,
            FileInfo fileInfo,
            IEnumerable<BankIn> data)
        {
            using (ExcelPackage xlPackage = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.First();

                // Set company name
                worksheet.Cells["C2"].Value = data.First().CompanyName;

                // Set month
                worksheet.Cells["C3"].Value = selectedTime.ToString("MMMM yyyy");

                // Set values for each banks
                var bankInDataGrouped = data.GroupBy(x => x.BankName);
                var startRow = 10;
                foreach (IGrouping<string, BankIn> bankIns in bankInDataGrouped)
                {
                    worksheet.Cells[startRow, 2].LoadFromCollectionFiltered(bankIns.ToList());

                    // Create sumary row for a bank
                    startRow = startRow + bankIns.Count();
                    var summaryRow = worksheet.Cells[startRow, 2, startRow, 5];
                    summaryRow.Merge = true;
                    summaryRow.Value = "Total (RM) - " + bankIns.Key;

                    worksheet.Cells[startRow, 6].FormulaR1C1 =
                        $"=SUM(R{(startRow - bankIns.Count() - 1)}C6:R{startRow - 1}C6)";

                    startRow++;
                }

                return xlPackage.GetAsByteArray();
            }
        }
    }
}