namespace HRP.Models.PaymentProcess
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Resources;
    using NPOI.SS.UserModel;

    public class AttendanceRecordReader : TemplateReader
    {
        public AttendanceRecordReader(Stream file) : base(file)
        {
            ValidationErrors = new List<ValidationResult>();
        }

        public override bool IsValid
        {
            get
            {
                return
#pragma warning disable S1067 // Expressions should not be too complex
                    Headers.Contains(AttendanceRecordColumn.IDNo.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceRecordColumn.ActualWorkingDay.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceRecordColumn.LeaveTaken.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceRecordColumn.UnpaidLeave.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceRecordColumn.LateInMinute.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceRecordColumn.EarlyOutMinute.ToLower(CultureInfo.InvariantCulture));
#pragma warning restore S1067 // Expressions should not be too complex
            }
        }

        public override bool IsValidRow(IRow row)
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

        public override int StartRow => 6;

        public override int HeaderRow => 5;

        public Result<IEnumerable<AttendanceRecordUploading>> ValidateAttendanceRecords(IEnumerable<string> employeeCodeList, string companyName)
        {
            if (IsValid)
            {
                var records = new List<AttendanceRecordUploading>();
                for (var i = StartRow; i <= Sheet.LastRowNum; i++)
                {
                    var row = Sheet.GetRow(i);

                    if (IsValidRow(row))
                    {
                        var record = new AttendanceRecordUploading
                        {
                            RowNumber = row.RowNum,
                            EmployeeCode = GetStringValueAtCell(AttendanceRecordColumn.IDNo, row),
                            ActualWorkingDay = GetStringValueAtCell(AttendanceRecordColumn.ActualWorkingDay, row),
                            LeaveTaken = GetStringValueAtCell(AttendanceRecordColumn.LeaveTaken, row),
                            UnpaidLeave = GetStringValueAtCell(AttendanceRecordColumn.UnpaidLeave, row),
                            LateInMinute = GetStringValueAtCell(AttendanceRecordColumn.LateInMinute, row),
                            EarlyOutMinute = GetStringValueAtCell(AttendanceRecordColumn.EarlyOutMinute, row)
                        };

                        CheckEmployeeCode(record.EmployeeCode, row.RowNum, employeeCodeList, companyName);
                        CheckActualWorkingDay(record.ActualWorkingDay, row.RowNum);
                        CheckLeaveTaken(record.LeaveTaken, row.RowNum);
                        CheckUnpaidLeave(record.UnpaidLeave, row.RowNum);
                        CheckLateInMinute(record.LateInMinute, row.RowNum, 26);
                        CheckEarlyOutMinute(record.EarlyOutMinute, row.RowNum, 26);

                        records.Add(record);
                    }
                }

                return new Success<IEnumerable<AttendanceRecordUploading>>(records);
            }

            return new Failure<IEnumerable<AttendanceRecordUploading>>(Message.IncorrectTemplate);
        }

        private string GetStringValueAtCell(string columnName, IRow row)
        {
            var value = (string)GetPropValue(columnName, row, typeof(string));

            return value;
        }

        #region Validation

        private void CheckEmployeeCode(string value, int rowNumber, IEnumerable<string> employeeCodeList, string companyName)
        {
            var columnName = AttendanceRecordColumn.IDNo;
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
            else
            {
                result.ResultType = 1;
            }

            if (!result.IsValid)
            {
                ValidationErrors.Add(result);
            }
        }

        private void CheckActualWorkingDay(string value, int rowNumber)
        {
            var columnName = AttendanceRecordColumn.ActualWorkingDay;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);
            if (!result.IsValid)
            {
                ValidationErrors.Add(result);
            }
        }

        private void CheckLeaveTaken(string value, int rowNumber)
        {
            var columnName = AttendanceRecordColumn.LeaveTaken;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);
            if (!result.IsValid)
            {
                ValidationErrors.Add(result);
            }
        }

        private void CheckUnpaidLeave(string value, int rowNumber)
        {
            var columnName = AttendanceRecordColumn.UnpaidLeave;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);
            if (!result.IsValid)
            {
                ValidationErrors.Add(result);
            }
        }

        private void CheckLateInMinute(string value, int rowNumber, double maxValue)
        {
            var columnName = AttendanceRecordColumn.LateInMinute;
            var result = CheckValidNumber(value, columnName, rowNumber, maxValue * 1140);
            if (!result.IsValid)
            {
                ValidationErrors.Add(result);
            }
        }

        private void CheckEarlyOutMinute(string value, int rowNumber, double maxValue)
        {
            var columnName = AttendanceRecordColumn.EarlyOutMinute;
            var result = CheckValidNumber(value, columnName, rowNumber, maxValue * 1140);
            if (!result.IsValid)
            {
                ValidationErrors.Add(result);
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
    }
}