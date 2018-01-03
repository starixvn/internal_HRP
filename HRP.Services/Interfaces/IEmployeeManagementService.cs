namespace HRP.Services.Interfaces
{
    using HRP.Models.EmployeeManagement;

    internal interface IEmployeeManagementService
    {
        void CreateEmployee(EmployeePersonalDetail employee);
    }
}