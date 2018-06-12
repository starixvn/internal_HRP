namespace HRP.Models.ServiceModels
{
    using System.Globalization;
    using System.Xml.Serialization;
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class MonthlyAllowanceDeductionModel
    {
        public int PayrollId { get; set; }

        public int Type { get; set; }

        public int EmployeeId { get; set; }

        [XmlIgnore]
        public decimal? NoOfDays { get; set; }

        [XMLName("NoOfDays")]
        public string XmlNoOfDays
        {
            get => NoOfDays.HasValue ? NoOfDays.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        public string UnitAmount { get; set; }

        public string TotalAmount { get; set; }

        [XmlIgnore]
        public string TotalAmountToDisplay { get; set; }

        public string NetAllowance { get; set; }

        [XmlIgnore]
        public string NetAllowanceToDisplay { get; set; }

        public int? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }
    }
}