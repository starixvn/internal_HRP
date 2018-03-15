namespace HRP.Models.MonthlyPayroll
{
    using System;

    public class MonthlyDSTNetSalary
    {
        public int EmployeeId { get; set; }

        public DateTime Month { get; set; }

        public int CurrencyId { get; set; }

        public string NetSalaryReceived { get; set; }
    }
}