namespace HRP.Services
{
    using System.Collections.Generic;
    using System.IO;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Models.PaymentProcess;
    using NPOI.SS.UserModel;

    public interface ITemplateReader
    {
        ISheet CreateSheet(Stream file);

        Result<IEnumerable<AttendanceUploading>> ReadAttendance(ISheet sheet, IEnumerable<string> employeeCodeList, string companyName);

        IList<ValidationResult> ValidationResults { get; }
    }
}