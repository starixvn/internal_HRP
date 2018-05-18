namespace HRP.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Core.Enumerations;
    using Models.Report;

    public interface IExportExcelService
    {
        byte[] ExportMonthlyPayrollReport(string templatePath, MonthlyPayrollReport report);

        byte[] ExportDirectBankingReports(DateTime selectedTime, FileInfo fileInfo, PaymentType typeOfPayment, BankInReport data);

        byte[] ExportEbankingReports(
            DateTime selectedTime,
            FileInfo fileInfo,
            PaymentType typeOfPayment,
            BankInReport data);

        byte[] ExportDynamicReport(IEnumerable<DynamicReportModel> records, IEnumerable<int> selectedIds);
    }
}