namespace HRP.Core.Model
{
    public class CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = "Invalid data";
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful => Code == 0;
    }
}