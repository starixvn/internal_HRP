namespace HRP.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Models.Report;

    public interface IExportExcelService
    {
        Stream GetPayDetails(Stream fileStream);

        Stream ExportDynamicReport(IEnumerable<DynamicReportModel> records, GenerateReportViewModel viewModel);

        byte[] ExportBankInReports(DateTime selectedTime, FileInfo fileInfo, IEnumerable<BankIn> data);
    }
}