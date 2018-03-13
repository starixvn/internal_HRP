namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;
    using Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class MonthlySalaryAdditionModel
    {
        public int PayrollId { get; set; }

        public int Type { get; set; }

        [XmlIgnore]
        public decimal? UnitAmount { get; set; }

        [XmlIgnore]
        public decimal? NoOfDays { get; set; }

        public string NetSalary { get; set; }

        public string Description { get; set; }

        [XMLName("UnitAmount")]
        public string XmlUnitAmount
        {
            get
            {
                var unitAmount = string.Empty;
                if (UnitAmount.HasValue)
                {
                    unitAmount = UnitAmount.Value.ToString(CultureInfo.InvariantCulture);
                }

                return unitAmount;
            }
        }

        [XMLName("NoOfDays")]
        public string XmlNoOfDays
        {
            get
            {
                var noOfDays = string.Empty;
                if (NoOfDays.HasValue)
                {
                    noOfDays = NoOfDays.Value.ToString(CultureInfo.InvariantCulture);
                }

                return noOfDays;
            }
        }
    }
}