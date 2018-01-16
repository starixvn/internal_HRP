namespace HRP.Website.Views.Shared.Components.CheckBoxList
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class CheckBoxListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<ReferenceResourceModel> datas)
        {
            var viewModel = new CheckBoxListViewModel { CheckBoxItems = new List<CheckBoxItem>() };
            
            foreach (var referenceResourceModel in datas)
            {
                var item = new CheckBoxItem
                {
                    Id = referenceResourceModel.ResourceId,
                    Value = referenceResourceModel.Value,
                    Text = referenceResourceModel.Value
                };

                viewModel.CheckBoxItems.Add(item);
            }

            return View("CheckBoxList", viewModel);
        }
    }

    public class CheckBoxListViewModel
    {
        // Simple check box list
        public IList<CheckBoxItem> CheckBoxItems { get; set; }

        public bool IsIncludeAll { get; set; }
    }

    public class CheckBoxItem
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public string Text { get; set; }

        public bool Selected { get; set; }
    }
}
