namespace HRP.Core.Extensions
{
    using HRP.Resources;
    using NPOI.SS.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public abstract class TemplateReader
    {
        private static DataFormatter _formatter = new DataFormatter();
        private ILookup<string, int> _headers;

        protected TemplateReader(Stream file)
        {
            Sheet = WorkbookFactory.Create(file).GetSheetAt(0);
        }

        protected ISheet Sheet { get; private set; }

        public abstract bool IsValid { get; }

        public abstract int StartRow { get; }

        public abstract int HeaderRow { get; }

        protected ILookup<string, int> Headers
        {
            get { return _headers = _headers ?? GetHeaders(); }
        }

        public virtual bool IsValidRow(IRow row)
        {
            return true;
        }

        public virtual Result<ISheet> GetValidSheet()
        {
            return IsValid ? (Result<ISheet>)new Success<ISheet>(Sheet) : (Result<ISheet>)new Failure<ISheet>(Message.IncorrectTemplate);
        }

        public virtual bool BasicType(Type t)
        {
            var tmp = t;

            if (t != null && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                tmp = t.GetGenericArguments()[0];
            }

            return (tmp?.IsPrimitive ?? tmp != default(Type)) || tmp == typeof(string) || tmp == typeof(DateTime);
        }

        public virtual Type GetBasicType(Type t)
        {
            var tmp = t;

            if (t != null && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                tmp = t.GetGenericArguments()[0];
            }

            return tmp;
        }

        public virtual object GetPropValue(string column, IRow row, Type rowType)
        {
            var t = GetBasicType(rowType);

            var values = Headers[column.ToLower(CultureInfo.InvariantCulture)].Select(idx => CellReader(row, idx, t));

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
            var value = _formatter.FormatCellValue(cell);

            return value ?? string.Empty;
        }

        public virtual object ReadDateTime(ICell cell)
        {
            return cell.DateCellValue;
        }

        private ILookup<string, int> GetHeaders()
        {
            return Sheet
                .GetRow(HeaderRow)
                .Cells
                .ToLookup(cell => cell.StringCellValue.ToLower(CultureInfo.InvariantCulture), cell => cell.ColumnIndex);
        }
    }
}