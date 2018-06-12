namespace HRP.Core.Extensions.TagHelpers
{
    using System;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("input", Attributes = ForAttributeName)]
    public class EncryptedDataTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-encrypt-for";

        public EncryptedDataTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlGenerator Generator { get; }

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var hidden = Generator.GenerateHidden(ViewContext, For.ModelExplorer, For.Name, For.Model, false, new { @class = "encrypted-value" });

            if (output.Attributes.TryGetAttribute("class", out TagHelperAttribute attribute))
            {
                output.Attributes.SetAttribute("class", $"{attribute.Value} plain-value");
            }
            else
            {
                output.Attributes.Add("class", "plain-value");
            }

            output.Attributes.Add("data-encrypt-target", $"#{hidden.Attributes["id"]}");
            hidden.Attributes.Add("data-plain-target", $"#{output.Attributes["id"].Value}");

            output.PostElement.SetHtmlContent(hidden);
        }
    }
}