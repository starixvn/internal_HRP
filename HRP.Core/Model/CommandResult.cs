namespace HRP.Core.Model
{
    public class CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = Resources.Message.InvalidData;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful => Code == 0;

        public object Data { get; set; }
    }
}