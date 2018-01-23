namespace HRP.Services
{
    using HRP.Models.EmployeeManagement;

    public interface IEmployeePayrollService
    {
        EmployeePayrollViewModel GetEmployeePayroll(int employeeId, int userId);
        void UpdatePayroll(EmployeePayrollViewModel model, int userId);
    }
}
