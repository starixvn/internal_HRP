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

    public class AttendanceReader : TemplateReader
    {
        public AttendanceReader(Stream file) : base(file)
        {
            ValidationResults = new List<ValidationResult>();
            AttendanceUploading = new List<AttendanceUploading>();
        }

        private IList<AttendanceUploading> AttendanceUploading { get; set; }

        public override bool IsValid
        {
            get
            {
                return
#pragma warning disable S1067 // Expressions should not be too complex
                    Headers.Contains(AttendanceColumn.IDNo.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceColumn.ActualWorkingDay.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceColumn.LeaveTaken.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceColumn.UnpaidLeave.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceColumn.LateInMinute.ToLower(CultureInfo.InvariantCulture)) &&
                    Headers.Contains(AttendanceColumn.EarlyOutMinute.ToLower(CultureInfo.InvariantCulture));
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

        public Result<IEnumerable<AttendanceUploading>> ReadAttendances(IEnumerable<string> employeeCodeList, string companyName)
        {
            if (IsValid)
            {
                for (var i = StartRow; i <= Sheet.LastRowNum; i++)
                {
                    var row = Sheet.GetRow(i);

                    if (IsValidRow(row))
                    {
                        var currentRowNumber = row.RowNum + 1;
                        var record = new AttendanceUploading
                        {
                            RowNumber = currentRowNumber,
                            EmployeeCode = GetStringValueAtCell(AttendanceColumn.IDNo, row),
                            ActualWorkingDay = GetStringValueAtCell(AttendanceColumn.ActualWorkingDay, row),
                            LeaveTaken = GetStringValueAtCell(AttendanceColumn.LeaveTaken, row),
                            UnpaidLeave = GetStringValueAtCell(AttendanceColumn.UnpaidLeave, row),
                            LateInMinute = GetStringValueAtCell(AttendanceColumn.LateInMinute, row),
                            EarlyOutMinute = GetStringValueAtCell(AttendanceColumn.EarlyOutMinute, row)
                        };

                        CheckEmployeeCode(record.EmployeeCode, currentRowNumber, employeeCodeList, companyName);
                        CheckActualWorkingDay(record.ActualWorkingDay, currentRowNumber);
                        CheckLeaveTaken(record.LeaveTaken, currentRowNumber);
                        CheckUnpaidLeave(record.UnpaidLeave, currentRowNumber);
                        CheckLateInMinute(record.LateInMinute, currentRowNumber, 26);
                        CheckEarlyOutMinute(record.EarlyOutMinute, currentRowNumber, 26);

                        AttendanceUploading.Add(record);
                    }
                }

                return new Success<IEnumerable<AttendanceUploading>>(AttendanceUploading);
            }

            return new Failure<IEnumerable<AttendanceUploading>>(Message.IncorrectTemplate);
        }

        private string GetStringValueAtCell(string columnName, IRow row)
        {
            var value = (string)GetPropValue(columnName, row, typeof(string));

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
            else if (AttendanceUploading.Any(s => s.EmployeeCode.Equals(value)))
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
                ValidationResults.Add(result);
            }
        }

        private void CheckActualWorkingDay(string value, int rowNumber)
        {
            var columnName = AttendanceColumn.ActualWorkingDay;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);
            if (!result.IsValid)
            {
                ValidationResults.Add(result);
            }
        }

        private void CheckLeaveTaken(string value, int rowNumber)
        {
            var columnName = AttendanceColumn.LeaveTaken;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);
            if (!result.IsValid)
            {
                ValidationResults.Add(result);
            }
        }

        private void CheckUnpaidLeave(string value, int rowNumber)
        {
            var columnName = AttendanceColumn.UnpaidLeave;
            var result = CheckValidNumber(value, columnName, rowNumber, 31);
            if (!result.IsValid)
            {
                ValidationResults.Add(result);
            }
        }

        private void CheckLateInMinute(string value, int rowNumber, double maxValue)
        {
            var columnName = AttendanceColumn.LateInMinute;
            var result = CheckValidNumber(value, columnName, rowNumber, maxValue * 1140);
            if (!result.IsValid)
            {
                ValidationResults.Add(result);
            }
        }

        private void CheckEarlyOutMinute(string value, int rowNumber, double maxValue)
        {
            var columnName = AttendanceColumn.EarlyOutMinute;
            var result = CheckValidNumber(value, columnName, rowNumber, maxValue * 1140);
            if (!result.IsValid)
            {
                ValidationResults.Add(result);
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