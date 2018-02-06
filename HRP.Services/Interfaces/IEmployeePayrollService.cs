namespace HRP.Services.Interfaces
{
    using HRP.Core.Model;
    using Models.EmployeeManagement;
    using Models.EmployeeManagement.Payroll;

    public interface IEmployeePayrollService
    {
        EmployeePayrollViewModel GetEmployeePayroll(int employeeId, int userId);
        CommandResult UpdatePayroll(EmployeePayrollViewModel model, int userId);
    }
}
