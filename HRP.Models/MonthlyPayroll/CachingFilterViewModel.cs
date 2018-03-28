namespace HRP.Models.MonthlyPayroll
{
    public class CachingFilterViewModel
    {
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public string IDNo { get; set; }

        public string CostCenterIds { get; set; }

        public int? WorkingDays { get; set; }
    }
}