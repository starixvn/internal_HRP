namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Report;

    public interface IReportService
    {
        Task<IEnumerable<BankIn>> GetBankInAsync(int companyId, DateTime selectedTime);
    }
}
