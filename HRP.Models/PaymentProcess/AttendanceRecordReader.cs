namespace HRP.Models.PaymentProcess
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using HRP.Core.Extensions;
    using HRP.Resources;
    using NPOI.SS.UserModel;

    public class AttendanceRecordReader : TemplateReader
    {
        public AttendanceRecordReader(Stream file) : base(file)
        {
        }

        public override bool IsValid
        {
            get
            {
                return
#pragma warning disable S1067 // Expressions should not be too complex
                    Headers.Contains("id no") &&
                    Headers.Contains("actual working day") &&
                    Headers.Contains("leave taken") &&
                    Headers.Contains("unpaid leave") &&
                    Headers.Contains("late in minute") &&
                    Headers.Contains("early out minute");
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

        public Result<IEnumerable<AttendanceRecord>> GetAttendanceRecords()
        {
            if (IsValid)
            {
                for (var i = StartRow; i <= Sheet.LastRowNum; i++)
                {
                    var row = Sheet.GetRow(i);

                    if (IsValidRow(row))
                    {
                        return new Success<IEnumerable<AttendanceRecord>>(new List<AttendanceRecord>());
                    }
                }
            }

            return new Failure<IEnumerable<AttendanceRecord>>(Message.IncorrectTemplate);
        }

        public string GetEmployeeCodeAtRow(IRow row)
        {
            var value = GetPropValue("id no", row, typeof(string));

            return (string)value;
        }

        public int GetActualWorkingDayAtRow(int rowNum)
        {
            var value = GetPropValue("actual working day", Sheet.GetRow(rowNum), typeof(int));

            return (int)value;
        }
    }
}