namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public abstract class BaseNonQueryCommand : NonQueryCommand
    {
        public virtual int ErrCode { get; set; }

        public virtual string ErrMsg { get; set; }
    }
}