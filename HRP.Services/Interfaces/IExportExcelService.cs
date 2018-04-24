namespace HRP.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Models.Report;

    public interface IExportExcelService
    {
        byte[] ExportMonthlyPayrollReport(string templatePath, MonthlyPayrollReport report);

        byte[] ExportBankInReports(DateTime selectedTime, FileInfo fileInfo, PaymentType typeOfPayment, BankInReport data);

        byte[] ExportDynamicReport(IEnumerable<DynamicReportModel> records, IEnumerable<int> selectedIds);
    }
}