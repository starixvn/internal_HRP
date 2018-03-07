namespace HRP.Core.Model
{
    public class ValidationResult
    {
        public int RowNumber { get; set; }

        public int ResultType { get; set; }

        public string Message { get; set; }

        public bool IsValid => ResultType == 1;
    }
}