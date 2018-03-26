namespace HRP.Services
{
    using System.IO;
    using HRP.Core.Extensions;
    using HRP.Models.PaymentProcess;
    using NPOI.SS.UserModel;

    public interface ITemplateReader
    {
        ISheet CreateSheet(Stream file);

        Result<AttendanceTemplate> ReadAttendance(AttendanceTemplate template);

        Result<DSTNetSalaryTemplate> ReadDSTNetSalary(DSTNetSalaryTemplate template);
    }
}