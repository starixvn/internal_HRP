namespace HRP.Models.MonthlyPayroll
{
    using System.Globalization;
    using System.Xml.Serialization;
    using HRP.Core.CustomValidations;
    using HRP.Core.Extensions;
    using HRP.Resources;

    [XMLName("Row")]
    public class MonthlyAllowanceAdditionModel
    {
        public int PayrollId { get; set; }

        public int Type { get; set; }

        public int EmployeeId { get; set; }

        [XMLName("NoOfDays")]
        public string XmlNoOfDays
        {
            get => (NoOfDays.HasValue) ? NoOfDays.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        [XmlIgnore]
        public decimal? NoOfDays { get; set; }

        public string TotalAmount { get; set; }

        [RequiredIfRealtedFieldHasValue(RelatedName = nameof(NoOfDays), ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "UnitAmountRequired")]
        public string UnitAmount { get; set; }

        public string NetAllowance { get; set; }

        [XmlIgnore]
        public string TotalAmountToDisplay { get; set; }

        public int? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        [XmlIgnore]
        public string NetAllowanceToDisplay { get; set; }
    }
}