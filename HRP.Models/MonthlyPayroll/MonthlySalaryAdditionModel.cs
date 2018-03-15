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

        public string UnitAmount { get; set; }

        [XmlIgnore]
        public decimal? NoOfDays { get; set; }

        public string NetSalary { get; set; }

        public string Description { get; set; }

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