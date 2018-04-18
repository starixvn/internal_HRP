namespace HRP.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MultiSelectViewModel
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; } = true;
        public bool Enable { get; set; } = true;
        public string CssClass { get; set; } = "-option-check";
        public string ParentId { get; set; }
        public IList<MultiSelectViewModel> Items { get; set; } = new List<MultiSelectViewModel>();
    }
}