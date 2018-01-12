namespace HRP.Models.EmployeeManagement
{
    public class EmployeePayrollAllowanceDetailModel
    {
        public int AllowanceTypeId { get; set; }

        public string Description { get; set; }

        public string CurrencyCode { get; set; }

        public string Amount { get; set; }
    }
}