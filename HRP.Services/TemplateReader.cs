namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Models.PaymentProcess;
    using HRP.Resources;
    using NPOI.SS.UserModel;

    public class TemplateReader : ITemplateReader
    {
        private readonly DataFormatter formatter;
        private readonly IList<AttendanceUploading> attendanceUploading;
        private readonly IList<ValidationResult> _validationResults;
        private const int StartRow = 6;
        private const int HeaderRow = 5;

        public TemplateReader(DataFormatter formatter = null)
        {
            attendanceUploading = new List<AttendanceUploading>();
            _validationResults = new List<ValidationResult>();
            this.formatter = formatter ?? new DataFormatter();
        }

        public ISheet CreateSheet(Stream file)
        {
            return WorkbookFactory.Create(file).GetSheetAt(0);
        }

        public static bool IsValidSheet(ISheet sheet)
        {
            var headers = GetHeaders(sheet);
            return
#pragma warning disable S1067 // Expressions should not be too complex
                    headers.Contains(AttendanceColumn.IDNo.ToLower(CultureInfo.InvariantCulture)) &&
                headers.Contains(AttendanceColumn.ActualWorkingDay.ToLower(CultureInfo.InvariantCulture)) &&
                headers.Contains(AttendanceColumn.LeaveTaken.ToLower(CultureInfo.InvariantCulture)) &&
                headers.Contains(AttendanceColumn.UnpaidLeave.ToLower(CultureInfo.InvariantCulture)) &&
                headers.Contains(AttendanceColumn.LateInMinute.ToLower(CultureInfo.InvariantCulture)) &&
                headers.Contains(AttendanceColumn.EarlyOutMinute.ToLower(CultureInfo.InvariantCulture));
#pragma warning restore S1067 // Expressions should not be too complex
        }

        public static bool IsValidRow(IRow row)
        {
            return !IsEmptyRow(row);
        }

        private static bool IsEmptyRow(IRow row)
        {
            return
                row == null
                || row.Cells.Count == 0
                || row.Cells.TrueForAll(c => c.CellType == CellType.Blank || (c.CellType == CellType.String && string.IsNullOrWhiteSpace(c.StringCellValue)));
        }

        public IList<ValidationResult> ValidationResults
        {
            get => _validationResults;
        }

        public Result<IEnumerable<AttendanceUploading>> ReadAttendance(ISheet sheet, IEnumerable<string> employeeCodeList, string companyName)
        {
            if (IsValidSheet(sheet))
            {
                for (var i = StartRow; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);

                    if (IsValidRow(row))
                    {
                        var currentRowNumber = row.RowNum + 1;
                        var record = new AttendanceUploading
                        {
                            RowNumber = currentRowNumber,
                            EmployeeCode = GetStringValueAtCell(AttendanceColumn.IDNo, sheet, row),
                            ActualWorkingDay = GetStringValueAtCell(AttendanceColumn.ActualWorkingDay, sheet, row),
                            LeaveTaken = GetStringValueAtCell(AttendanceColumn.LeaveTaken, sheet, row),
                            UnpaidLeave = GetStringValueAtCell(AttendanceColumn.UnpaidLeave, sheet, row),
                            LateInMinute = GetStringValueAtCell(AttendanceColumn.LateInMinute, sheet, row),
                            EarlyOutMinute = GetStringValueAtCell(AttendanceColumn.EarlyOutMinute, sheet, row)
                        };

                        CheckEmployeeCode(record.EmployeeCode, currentRowNumber, employeeCodeList, companyName);
                        CheckActualWorkingDay(record.ActualWorkingDay, currentRowNumber);
                        CheckLeaveTaken(record.LeaveTaken, currentRowNumber);
                        CheckUnpaidLeave(record.UnpaidLeave, currentRowNumber);
                        CheckLateInMinute(record.LateInMinute, currentRowNumber, 26);
                        CheckEarlyOutMinute(record.EarlyOutMinute, currentRowNumber, 26);

                        attendanceUploading.Add(record);
                    }
                }

                return new Success<IEnumerable<AttendanceUploading>>(attendanceUploading);
            }

            return new Failure<IEnumerable<AttendanceUploading>>(Message.IncorrectTemplate);
        }

        private string GetStringValueAtCell(string columnName, ISheet sheet, IRow row)
        {
            var value = (string)GetPropValue(columnName, sheet, row, typeof(string));

            return value;
        }

        #region Validation

        private void CheckEmployeeCode(string value, int rowNumber, IEnumerable<string> employeeCodeList, string companyName)
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
            else if (!employeeCodeList.Any(s => s.Equals(value)))
            {
                result.ResultType = 2;
                result.Message = string.Format(Message.IDNoNotExistIn, companyName);
            }
            else if (attendanceUploading.Any(s => s.EmployeeCode.Equals(value)))
            {
                result.ResultType = 2;
                result.Message = Message.IDNoIsDuplicated;
            }
            else
            {
                result.ResultType = 1;
            }

            if (!result.IsValid)
            {
                _validationResults.Add(result);
            }
        }

        private void CheckActualWorkingDay(string value, int rowNumber)
        {
            var columnName = AttendanceColumn.ActualWorkingDay;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);
            if (!result.IsValid)
            {
                _validationResults.Add(result);
            }
        }

        private void CheckLeaveTaken(string value, int rowNumber)
        {
            var columnName = AttendanceColumn.LeaveTaken;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);
            if (!result.IsValid)
            {
                _validationResults.Add(result);
            }
        }

        private void CheckUnpaidLeave(string value, int rowNumber)
        {
            var columnName = AttendanceColumn.UnpaidLeave;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);
            if (!result.IsValid)
            {
                _validationResults.Add(result);
            }
        }

        private void CheckLateInMinute(string value, int rowNumber, double maxValue)
        {
            var columnName = AttendanceColumn.LateInMinute;
            var result = CheckValidNumber(value, columnName, rowNumber, maxValue * 1140);
            if (!result.IsValid)
            {
                _validationResults.Add(result);
            }
        }

        private void CheckEarlyOutMinute(string value, int rowNumber, double maxValue)
        {
            var columnName = AttendanceColumn.EarlyOutMinute;
            var result = CheckValidNumber(value, columnName, rowNumber, maxValue * 1140);
            if (!result.IsValid)
            {
                _validationResults.Add(result);
            }
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

        public virtual Type GetBasicType(Type t)
        {
            var tmp = t;

            if (t != null && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                tmp = t.GetGenericArguments()[0];
            }

            return tmp;
        }

        public object GetPropValue(string column, ISheet sheet, IRow row, Type rowType)
        {
            var t = GetBasicType(rowType);

            var headers = GetHeaders(sheet);
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

        public virtual object ReadInt(ICell cell)
        {
            if (cell == null)
            {
                return default(int);
            }

            return cell.CellType == CellType.String ? int.Parse(cell.StringCellValue, CultureInfo.InvariantCulture) : (int)cell.NumericCellValue;
        }

        public virtual object ReadDouble(ICell cell)
        {
            if (cell == null)
            {
                return default(double);
            }

            return cell.CellType == CellType.String ? double.Parse(cell.StringCellValue, CultureInfo.InvariantCulture) : cell.NumericCellValue;
        }

        public virtual object ReadString(ICell cell)
        {
            var value = formatter.FormatCellValue(cell);

            return value ?? string.Empty;
        }

        public virtual object ReadDateTime(ICell cell)
        {
            return cell.DateCellValue;
        }

        private static ILookup<string, int> GetHeaders(ISheet sheet)
        {
            return sheet
                .GetRow(HeaderRow)
                .Cells
                .ToLookup(cell => cell.StringCellValue.ToLower(CultureInfo.InvariantCulture), cell => cell.ColumnIndex);
        }
    }
}