namespace HRP.Services
{
    using System;
    using System.Threading.Tasks;

    public interface IReportService
    {
        Task<byte[]> ExportBankInAsync(int companyId, DateTime selectedTime, string templateFileString);
    }
}
