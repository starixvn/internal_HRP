namespace HRP.Models
{
    using System.Collections.Generic;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class CommonSelectList
    {
        public static IList<SelectListItem> IsActive
        {
            get
            {
                var activeSelectList = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = string.Empty,
                        Text = ViewLang.All,
                        Selected = true
                    },
                    new SelectListItem
                    {
                        Value = true.ToString(),
                        Text = ViewLang.Yes
                    },
                    new SelectListItem
                    {
                        Value = false.ToString(),
                        Text = ViewLang.No
                    }
                };

                return activeSelectList;
            }
        }
    }
}
