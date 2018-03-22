namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class MonthlyPayroll
    {
        public int EmployeeId { get; set; }

        public DateTime Month { get; set; }

        public int Status { get; set; }

        [XmlIgnore]
        public DateTime? SalaryEffectiveDate { get; set; }

        [XMLName("SalaryEffectiveDate")]
        public string XmlSalaryEffectiveDate
        {
            get => (SalaryEffectiveDate.HasValue) ? SalaryEffectiveDate.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        public int? SalaryChangeReasonId { get; set; }

        public DateTime GroupStartDate { get; set; }

        [XmlIgnore]
        public DateTime? GroupEndDate { get; set; }

        [XMLName("GroupEndDate")]
        public string XmlGroupEndDate
        {
            get
            {
                var value = string.Empty;
                if (GroupEndDate.HasValue)
                {
                    value = GroupEndDate.Value.ToString(CultureInfo.InvariantCulture);
                }

                return value;
            }
        }
    }
}