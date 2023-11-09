namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class DivisionTableViewModel
    {
        public IEnumerable<Division> Divisions { get; set; }

        public Field Field { get; set; }

        public bool IsHost { get; set; }
    }
}