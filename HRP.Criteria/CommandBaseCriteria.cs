namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public abstract class CommandBaseCriteria : CriteriaBase
    {
        public virtual int ErrCode { get; set; }

        public virtual string ErrMsg { get; set; }
    }
}