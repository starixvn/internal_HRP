namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Xml.Serialization;
    using Core.Extensions;
    using HRP.Resources;

    [Serializable]
    [XMLName("Row")]
    public class MonthlySalaryDeductionModel
    {
        public int PayrollId { get; set; }

        public int Type { get; set; }

        public string UnitAmount { get; set; }

        [XmlIgnore]
        public decimal? NoOfDays { get; set; }

        public string NetSalary { get; set; }

        public string NetSalaryDisplay { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DescriptionMaxLength))]
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