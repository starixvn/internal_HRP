namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public abstract class HRPCriteriaBase : CriteriaBase
    {
        public int ErrCode { get; set; }
        
        public string ErrMsg { get; set; }
    }
}
