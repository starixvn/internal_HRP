namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Core.Enumerations;
    using Models.Report;

    public interface IExportExcelService
    {
        byte[] ExportDirectBankingReports(
            DateTime selectedTime,
            FileInfo fileInfo,
            PaymentType typeOfPayment,
            BankInReport data,
            string password = null);

        byte[] ExportEbankingReports(
            DateTime selectedTime,
            FileInfo fileInfo,
            PaymentType typeOfPayment,
            BankInReport data,
            string password = null);

        byte[] ExportDynamicReport(IEnumerable<DynamicReportModel> records, IEnumerable<int> selectedIds, string password = null);
    }
}