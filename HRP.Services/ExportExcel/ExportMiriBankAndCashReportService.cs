namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Models.Report;
    using HRP.Resources;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public interface IExportMiriBankAndCashReportService
    {
        byte[] Export(
            string templatePath,
            DateTime month,
            PaymentType typeOfPayment,
            DirectBankingReport report,
            string password = null);
    }

    public class ExportMiriBankAndCashReportService : IExportMiriBankAndCashReportService
    {
        private const short TotalCols = 7;
        private const short DirectBankingRowIndex = 10;
        private const short DirectBanking_PaymentTransaction_RowIndex = 11;
        private const short CashRowIndex = 12;
        private const short Cash_PaymentTransaction_RowIndex = 13;
        private const short RecordRowIndex = 14;
        private const short CurrencyCodeColIndex = 6;

        public byte[] Export(
            string templatePath,
            DateTime month,
            PaymentType typeOfPayment,
            DirectBankingReport report,
            string password = null)
        {
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Miri Bank & Cash");

                    WriteHeader(template, worksheet, month, typeOfPayment, report.CompanyName, report.PaymentTransactions);

                    int rowIndex = 10;
                    if (report.IsExportable)
                    {
                        rowIndex = WriteDataDirectBankingTransaction(template, worksheet, rowIndex, report.DirectBankingBankIns);
                        rowIndex++;
                        rowIndex = WriteDataCashTransaction(template, worksheet, rowIndex, report.CashBankIns);

                        rowIndex++;
                        rowIndex = WriteSummary(worksheet, rowIndex, Message.GrandTotalBankIn, report.DirectBankingBankIns, ColorTranslator.FromHtml("#e2f0d9"));
                        rowIndex++;
                        WriteSummary(worksheet, rowIndex, Message.GrandTotalCash, report.CashBankIns, ColorTranslator.FromHtml("#deebf7"));
                    }
                    else
                    {
                        WriteNoData(template, worksheet, rowIndex);
                    }

                    worksheet.Cells.AutoFitColumns();

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        package.Encryption.Password = password;
                    }

                    return package.GetAsByteArray();
                }
            }
        }

        private static void WriteHeader(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            DateTime month,
            PaymentType typeOfPayment,
            string company,
            IEnumerable<ReferenceResourceModel> paymentTransactions)
        {
            const short HeaderRowIndex = 9;
            template.Cells[1, 1, HeaderRowIndex, TotalCols].Copy(worksheet.Cells[1, 1, HeaderRowIndex, TotalCols]);

            worksheet.Cells["C2"].Value = company;
            worksheet.Cells["C3"].Value = month.ToString("MM-yyyy");
            worksheet.Cells["C4"].Value = typeOfPayment != 0 ? typeOfPayment.GetDisplayName() : PaymentType.BasicSalary.GetDisplayName();
            worksheet.Cells["C5"].Value = string.Join(",", paymentTransactions.Select(x => x.Value));
        }

        private static void WriteNoData(ExcelWorksheet template, ExcelWorksheet worksheet, int rowIndex)
        {
            const short NoDataRowIndex = 15;
            template.Cells[NoDataRowIndex, 1, NoDataRowIndex, TotalCols].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalCols]);
            worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Merge = true;
            worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private static int WriteDataDirectBankingTransaction(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            int rowIndex,
            IEnumerable<BankIn> bankIns)
        {
            if (bankIns != null && !bankIns.Any())
            {
                return rowIndex;
            }

            template.Cells[DirectBankingRowIndex, 1, DirectBankingRowIndex, TotalCols].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalCols]);
            worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Merge = true;
            var data = bankIns.GroupBy(x => new { x.PaymentTransactionId, x.PaymentTransaction }).OrderBy(x => x.Key.PaymentTransaction);
            rowIndex++;
            foreach (var item in data)
            {
                template.Cells[DirectBanking_PaymentTransaction_RowIndex, 1, DirectBanking_PaymentTransaction_RowIndex, TotalCols]
                    .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalCols]);
                worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Merge = true;
                worksheet.Cells[rowIndex, 1].Value = item.Key.PaymentTransaction;
                rowIndex++;

                IEnumerable<BankIn> bankInSorted = item.OrderBy(x => x.BankName).ThenBy(x => x.CurrencyCode).ToList();
                foreach (var bankIn in bankInSorted)
                {
                    WriteRecord(template, worksheet, rowIndex, bankIn);

                    rowIndex++;
                }
            }

            return rowIndex;
        }

        private static int WriteDataCashTransaction(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            int rowIndex,
            IEnumerable<BankIn> bankIns)
        {
            if (bankIns != null && !bankIns.Any())
            {
                return rowIndex;
            }

            template.Cells[CashRowIndex, 1, CashRowIndex, TotalCols].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalCols]);
            worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Merge = true;
            var data = bankIns.GroupBy(x => new { x.PaymentTransactionId, x.PaymentTransaction }).OrderBy(x => x.Key.PaymentTransaction);
            rowIndex++;
            foreach (var item in data)
            {
                template.Cells[Cash_PaymentTransaction_RowIndex, 1, Cash_PaymentTransaction_RowIndex, TotalCols]
                    .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalCols]);
                worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Merge = true;
                worksheet.Cells[rowIndex, 1].Value = item.Key.PaymentTransaction;
                rowIndex++;

                IEnumerable<BankIn> bankInSorted = item.OrderBy(x => x.BankName).ThenBy(x => x.CurrencyCode).ToList();
                foreach (var bankIn in bankInSorted)
                {
                    WriteRecord(template, worksheet, rowIndex, bankIn);

                    rowIndex++;
                }
            }

            return rowIndex;
        }

        private static void WriteRecord(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            int rowIndex,
            BankIn bankIn)
        {
            short colIndex = 1;
            template.Cells[RecordRowIndex, 1, RecordRowIndex, TotalCols].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalCols]);
            worksheet.Cells[rowIndex, colIndex++].Value = bankIn.IDNo;
            worksheet.Cells[rowIndex, colIndex++].Value = bankIn.Nickname;
            worksheet.Cells[rowIndex, colIndex++].Value = bankIn.BeneficiaryName;
            worksheet.Cells[rowIndex, colIndex++].Value = bankIn.BankName;
            worksheet.Cells[rowIndex, colIndex++].Value = bankIn.AccountNumber;
            worksheet.Cells[rowIndex, colIndex++].Value = bankIn.CurrencyCode;

            worksheet.Cells[rowIndex, colIndex].Value = bankIn.AmountDecrypted;
            worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = Constants.NumberFormatNoDecimal;
        }

        private static int WriteSummary(
            ExcelWorksheet worksheet,
            int rowIndex,
            string summaryText,
            IEnumerable<BankIn> bankIns,
            Color backgroundColor)
        {
            if (bankIns != null && !bankIns.Any())
            {
                return rowIndex;
            }

            var beginRow = rowIndex;
            var bankInCurrencies = bankIns.GroupBy(x => x.CurrencyCode);
            foreach (IGrouping<string, BankIn> currency in bankInCurrencies)
            {
                worksheet.Cells[rowIndex, CurrencyCodeColIndex].Value = currency.Key;
                worksheet.Cells[rowIndex, CurrencyCodeColIndex + 1].Value = currency.Sum(x => x.AmountDecrypted);
                worksheet.Cells[rowIndex, CurrencyCodeColIndex + 1].Style.Numberformat.Format = Constants.NumberFormatNoDecimal;
                rowIndex++;
            }

            worksheet.Cells[beginRow, 1, rowIndex - 1, CurrencyCodeColIndex - 1].Merge = true;
            worksheet.Cells[beginRow, 1].Value = summaryText;
            worksheet.Cells[beginRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Fill.BackgroundColor.SetColor(backgroundColor);

            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            const string borderColor = "#a6a6a6";
            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Border.Top.Color.SetColor(ColorTranslator.FromHtml(borderColor));
            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Border.Left.Color.SetColor(ColorTranslator.FromHtml(borderColor));
            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Border.Bottom.Color.SetColor(ColorTranslator.FromHtml(borderColor));
            worksheet.Cells[beginRow, 1, rowIndex - 1, TotalCols].Style.Border.Right.Color.SetColor(ColorTranslator.FromHtml(borderColor));

            return rowIndex - 1;
        }
    }
}