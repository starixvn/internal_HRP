namespace HRP.Models.PaymentProcess
{
    using System.IO;
    using HRP.Core.Extensions;

    public class AttendanceReader : TemplateReader
    {
        public AttendanceReader(Stream file) : base(file)
        {
        }

        public override bool IsValid => throw new System.NotImplementedException();

        public override int StartRow => throw new System.NotImplementedException();

        public override int HeaderRow => throw new System.NotImplementedException();
    }
}