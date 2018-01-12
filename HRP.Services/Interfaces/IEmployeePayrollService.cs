namespace HRP.Services.Interfaces
{
    using HRP.Models.EmployeeManagement;

    public interface IEmployeePayrollService
    {
        EmployeePayrollViewModel GetEmployeePayroll(int employeeId);
    }
}
