namespace HRP.Services
{
    using System.Collections.Generic;
    using System.IO;
    using HRP.Core.Extensions;
    using HRP.Models.PaymentProcess;
    using NPOI.SS.UserModel;

    public interface ITemplateReader
    {
        ISheet CreateSheet(Stream file);

        Result<IEnumerable<AttendanceUploading>> ReadAttendance(AttendanceTemplate template);

        IList<ValidationResult> AttendanceValidationResults { get; }

        Result<IEnumerable<DSTNetSalaryUploading>> ReadDSTNetSalary(DSTNetSalaryTemplate template);

        IList<ValidationResult> DSTNetSalaryValidationResults { get; }
    }
}