using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRP.Models
{
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
                        Value = "",
                        Text = "All",
                        Selected = true
                    },
                    new SelectListItem
                    {
                        Value = true.ToString(),
                        Text = "Yes"
                    },
                    new SelectListItem
                    {
                        Value = false.ToString(),
                        Text = "No"
                    }
                };

                return activeSelectList;
            }
        }
    }
}
