namespace HRP.Models.Report
{
    using System;

    public class ConditionalCellStyle
    {
        public ConditionalCellStyle()
        {
        }

        public ConditionalCellStyle(ConditionalCellStyle original)
        {
            ColumnIndex = original.ColumnIndex;
            Condition = original.Condition;
            StyleIndex = original.StyleIndex;
            DataType = original.DataType;
        }

        public int ColumnIndex { get; set; }

        public Func<object, bool> Condition { get; set; }

        public Func<bool> RowCondition { get; set; }

        public int StyleIndex { get; set; }

        public Type DataType { get; set; }
    }
}
