namespace HRP.Models.DataConfiguration.Division
{
    using System.Collections.Generic;
    using HRP.Models;

    public class DivisionTableViewModel
    {
        public IEnumerable<Division> Divisions { get; set; }

        public Field Field { get; set; }

        public bool IsHost { get; set; }
    }
}