namespace HRP.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Models.Report;

    public interface IExportExcelService
    {
        Stream ExportDynamicReport(IEnumerable<DynamicReportModel> records, GenerateReportViewModel viewModel);

        byte[] ExportMonthlyPayrollReport(string templatePath, MonthlyPayrollReport report);

        byte[] ExportBankInReports(DateTime selectedTime, FileInfo fileInfo, PaymentType typeOfPayment, BankInReport data);

        byte[] ExportDynamicReport(IEnumerable<DynamicReportEmployeeModel> records, IEnumerable<DynamicReportColumnModel> columns);
    }
}