namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Extensions;
    using HRP.Core.Services;
    using HRP.Models.Currency;
    using HRP.Models.PaymentProcess;
    using HRP.Resources;
    using NPOI.SS.UserModel;

    public class TemplateReader : ITemplateReader
    {
        private readonly ICryptoService cryptoService;
        private readonly DataFormatter formatter;
        private readonly IList<AttendanceUploading> attendanceUploading;
        private readonly IList<ValidationResult> _attendanceValidationResults;
        private const int AttendanceStartRow = 6;
        private const int AttendanceHeaderRow = 5;

        private readonly IList<DSTNetSalaryUploading> dstNetSalaryUploading;
        private readonly IList<ValidationResult> _dstNetSalaryValidationResults;
        private const int DSTNetSalaryStartRow = 5;
        private const int DSTNetSalaryHeaderRow = 4;

        public TemplateReader(ICryptoService cryptoService, DataFormatter formatter = null)
        {
            this.cryptoService = cryptoService;
            this.formatter = formatter ?? new DataFormatter();
            attendanceUploading = new List<AttendanceUploading>();
            _attendanceValidationResults = new List<ValidationResult>();
            dstNetSalaryUploading = new List<DSTNetSalaryUploading>();
            _dstNetSalaryValidationResults = new List<ValidationResult>();
        }

        public ISheet CreateSheet(Stream file)
        {
            return WorkbookFactory.Create(file).GetSheetAt(0);
        }

        #region ReadAttendance

        public Result<AttendanceTemplate> ReadAttendance(AttendanceTemplate template)
        {
            try
            {
                var headers = GetHeaders(template.Sheet, AttendanceHeaderRow);
                if (!IsValidAttendanceSheet(headers))
                {
                    return new Failure<AttendanceTemplate>(Message.IncorrectTemplate);
                }

                for (var i = AttendanceStartRow; i <= template.Sheet.LastRowNum; i++)
                {
                    var row = template.Sheet.GetRow(i);

                    if (IsValidRow(row))

                    {
                        var currentRowNumber = row.RowNum + 1;
                        var record = new AttendanceUploading
                        {
                            RowNumber = currentRowNumber,
                            EmployeeCode = GetStringValueAtCell(AttendanceColumn.IDNo, headers, row),
                            ActualWorkingDay = GetStringValueAtCell(AttendanceColumn.ActualWorkingDay, headers, row)
                        };

                        var checkEmployeeCode = CheckEmployeeCode(record.EmployeeCode, currentRowNumber, template.CompanyCode, template.EmployeeCodeList, attendanceUploading);
                        var checkActualWorkingDay = CheckActualWorkingDay(record.ActualWorkingDay, currentRowNumber);

                        AddToAttendanceValidationResults(checkEmployeeCode);
                        AddToAttendanceValidationResults(checkActualWorkingDay);

                        attendanceUploading.Add(record);
                    }
                }

                template.Records = attendanceUploading;
                template.ValidationResults = _attendanceValidationResults;

                return new Success<AttendanceTemplate>(template);
            }
            catch (Exception)
            {
                return new Failure<AttendanceTemplate>(Message.ErrorOccuredWhileReadingTemplate);
            }
        }

        private static bool IsValidAttendanceSheet(ILookup<string, int> headers)
        {
            return
                    headers.Contains(AttendanceColumn.IDNo.ToLower(CultureInfo.InvariantCulture)) &&
                headers.Contains(AttendanceColumn.ActualWorkingDay.ToLower(CultureInfo.InvariantCulture));
        }

        private void AddToAttendanceValidationResults(ValidationResult result)
        {
            if (!result.IsValid)
            {
                _attendanceValidationResults.Add(result);
            }
        }

        #endregion ReadAttendance

        #region ReadDSTNetSalary

        public Result<DSTNetSalaryTemplate> ReadDSTNetSalary(DSTNetSalaryTemplate template)

        {
            try
            {
                var headers = GetHeaders(template.Sheet, DSTNetSalaryHeaderRow);
                if (!IsValidDSTNetSalarySheet(headers))
                {
                    return new Failure<DSTNetSalaryTemplate>(Message.IncorrectTemplate);
                }

                for (var i = DSTNetSalaryStartRow; i <= template.Sheet.LastRowNum; i++)
                {
                    var row = template.Sheet.GetRow(i);

                    if (IsValidRow(row))
                    {
                        var currentRowNumber = row.RowNum + 1;
                        var record = new DSTNetSalaryUploading
                        {
                            RowNumber = currentRowNumber,
                            EmployeeCode = GetStringValueAtCell(DSTNetSalaryColumn.IDNo, headers, row),
                            Currency = GetStringValueAtCell(DSTNetSalaryColumn.Currency, headers, row),
                            NetSalaryReceived = GetStringValueAtCell(DSTNetSalaryColumn.NetSalaryReceived, headers, row)
                        };

                        var checkEmployeeCode = CheckEmployeeCode(record.EmployeeCode, currentRowNumber, template.CompanyCode, template.EmployeeCodeList, dstNetSalaryUploading);
                        var checkCurrency = CheckCurrency(record.Currency, currentRowNumber, template.Currencies);
                        var checkNetSalaryReceived = CheckNetSalaryReceived(record.NetSalaryReceived, currentRowNumber);
#pragma warning disable S134 // Control flow statements "if", "switch", "for", "foreach", "while", "do"  and "try" should not be nested too deeply
                        if (checkNetSalaryReceived.IsValid)
#pragma warning restore S134 // Control flow statements "if", "switch", "for", "foreach", "while", "do"  and "try" should not be nested too deeply
                        {
                            decimal.TryParse(record.NetSalaryReceived, out decimal salary);
                            salary = record.Currency.ToLower(CultureInfo.InvariantCulture).Equals("vnd") ? Math.Round(salary) : Math.Round(salary, 2);
                            record.NetSalaryReceived = salary.ToString(CultureInfo.InvariantCulture);
                        }

                        AddToDSTNetSalaryValidationResults(checkEmployeeCode);
                        AddToDSTNetSalaryValidationResults(checkCurrency);
                        AddToDSTNetSalaryValidationResults(checkNetSalaryReceived);

                        record.NetSalaryReceived = cryptoService.Encrypt(record.NetSalaryReceived);
                        dstNetSalaryUploading.Add(record);
                    }
                }

                template.Records = dstNetSalaryUploading;
                template.ValidationResults = _dstNetSalaryValidationResults;

                return new Success<DSTNetSalaryTemplate>(template);
            }
            catch (Exception)
            {
                return new Failure<DSTNetSalaryTemplate>(Message.ErrorOccuredWhileReadingTemplate);
            }
        }

        private static bool IsValidDSTNetSalarySheet(ILookup<string, int> headers)
        {
            return
                    headers.Contains(DSTNetSalaryColumn.IDNo.ToLower(CultureInfo.InvariantCulture)) &&
                headers.Contains(DSTNetSalaryColumn.Currency.ToLower(CultureInfo.InvariantCulture)) &&
                headers.Contains(DSTNetSalaryColumn.NetSalaryReceived.ToLower(CultureInfo.InvariantCulture));
        }

        private void AddToDSTNetSalaryValidationResults(ValidationResult result)
        {
            if (!result.IsValid)
            {
                _dstNetSalaryValidationResults.Add(result);
            }
        }

        #endregion ReadDSTNetSalary

        #region Validation

        private ValidationResult CheckCurrency(string value, int rowNumber, IEnumerable<Currency> currencies)
        {
            var columnName = DSTNetSalaryColumn.Currency;
            var result = new ValidationResult
            {
                RowNumber = rowNumber
            };

            if (string.IsNullOrWhiteSpace(value))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.ValueMissing, columnName);
            }
            else if (!currencies.Any(s => string.Equals(value, s.CurrencyCode, StringComparison.OrdinalIgnoreCase)))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.CurrencyNotExists);
            }
            else
            {
                result.ResultType = 1;
            }

            return result;
        }

        private ValidationResult CheckNetSalaryReceived(string value, int rowNumber)
        {
            var columnName = DSTNetSalaryColumn.NetSalaryReceived;
            var result = new ValidationResult
            {
                RowNumber = rowNumber
            };

            if (string.IsNullOrWhiteSpace(value))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.ValueMissing, columnName);
            }
            else if (!decimal.TryParse(value, out decimal number))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.ValueMustBeANumber, columnName);
            }
            else
            {
                result.ResultType = 1;
            }

            return result;
        }

        private ValidationResult CheckEmployeeCode(string value, int rowNumber, string companyName,
                                                                IEnumerable<string> employeeCodeList,
                                                                IEnumerable<TemplateUploading> checkedRecords)
        {
            var columnName = AttendanceColumn.IDNo;
            var result = new ValidationResult
            {
                RowNumber = rowNumber
            };

            if (string.IsNullOrWhiteSpace(value))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.ValueMissing, columnName);
            }
            else if (!employeeCodeList.Any(s => string.Equals(value, s, StringComparison.OrdinalIgnoreCase)))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.IDNoNotExistIn, companyName);
            }
            else if (checkedRecords.Any(s => string.Equals(value, s.EmployeeCode, StringComparison.OrdinalIgnoreCase)))
            {
                result.ResultType = 2;
                result.Message = Message.IDNoIsDuplicated;
            }
            else
            {
                result.ResultType = 1;
            }

            return result;
        }

        private ValidationResult CheckActualWorkingDay(string value, int rowNumber)
        {
            var columnName = AttendanceColumn.ActualWorkingDay;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);

            return result;
        }

        private ValidationResult CheckValidNumber(string value, string columnName, int rowNumber, double maxValue)
        {
            var result = new ValidationResult
            {
                RowNumber = rowNumber
            };

            if (string.IsNullOrWhiteSpace(value))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.ValueMissing, columnName);
            }
            else if (!double.TryParse(value, out double number))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.ValueMustBeANumber, columnName);
            }
            else if (!(number >= 0 && number <= maxValue))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.ValueMustBeGreaterAndNotExceed, columnName, maxValue);
            }
            else
            {
                result.ResultType = 1;
            }

            return result;
        }

        #endregion Validation

        #region Helper

        private static ILookup<string, int> GetHeaders(ISheet sheet, int headerRow)
        {
            return sheet
                .GetRow(headerRow)
                .Cells
                .ToLookup(cell => cell.StringCellValue.ToLower(CultureInfo.InvariantCulture), cell => cell.ColumnIndex);
        }

        private static bool IsValidRow(IRow row)
        {
            return !IsEmptyRow(row);
        }

        private static bool IsEmptyRow(IRow row)
        {
            return
                row == null
                || row.Cells.TrueForAll(c => c.CellType == CellType.Blank || (c.CellType == CellType.String && string.IsNullOrWhiteSpace(c.StringCellValue)))
                || row.Cells.Count == 0;
        }

        private static Type GetBasicType(Type t)
        {
            var tmp = t;

            if (t != null && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                tmp = t.GetGenericArguments()[0];
            }

            return tmp;
        }

        private object GetPropValue(string column, ILookup<string, int> headers, IRow row, Type rowType)
        {
            var t = GetBasicType(rowType);
            var values = headers[column.ToLower(CultureInfo.InvariantCulture)].Select(idx => CellReader(row, idx, t));

            if (rowType.IsGenericType)
            {
                if (t == typeof(int))
                {
                    return values.Cast<int>();
                }

                if (t == typeof(double))
                {
                    return values.Cast<double>();
                }

                if (t == typeof(DateTime))
                {
                    return values.Cast<DateTime>();
                }

                return values.Cast<string>();
            }

            return values.First();
        }

        private object CellReader(IRow row, int idx, Type t)
        {
            var cell = row.GetCell(idx);

            if (t == typeof(int))
            {
                return ReadInt(cell);
            }

            if (t == typeof(double))
            {
                return ReadDouble(cell);
            }

            if (t == typeof(DateTime))
            {
                return ReadDateTime(cell);
            }

            return ReadString(cell);
        }

        private static object ReadInt(ICell cell)
        {
            if (cell == null)
            {
                return default(int);
            }

            return cell.CellType == CellType.String ? int.Parse(cell.StringCellValue, CultureInfo.InvariantCulture) : (int)cell.NumericCellValue;
        }

        private static object ReadDouble(ICell cell)
        {
            if (cell == null)
            {
                return default(double);
            }

            return cell.CellType == CellType.String ? double.Parse(cell.StringCellValue, CultureInfo.InvariantCulture) : cell.NumericCellValue;
        }

        private object ReadString(ICell cell)
        {
            var value = formatter.FormatCellValue(cell);

            return value ?? string.Empty;
        }

        private static object ReadDateTime(ICell cell)
        {
            return cell.DateCellValue;
        }

        private string GetStringValueAtCell(string columnName, ILookup<string, int> headers, IRow row)
        {
            var value = (string)GetPropValue(columnName, headers, row, typeof(string));

            return value;
        }

        #endregion Helper
    }
}