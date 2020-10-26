namespace HRP.Core.Extensions.TagHelpers
{
    using System;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.TagHelpers;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    // Source code for label tag helper: Mvc/src/Microsoft.AspNetCore.Mvc.TagHelpers/LabelTagHelper.cs
    // https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.TagHelpers/LabelTagHelper.cs

    [HtmlTargetElement("requiredlabel", Attributes = ForAttributeName)]
    public class RequiredLabelTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const int Thousand = 1000;

        /// <summary>
        /// Creates a new <see cref="LabelTagHelper"/>.
        /// </summary>
        /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
        public RequiredLabelTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        /// <inheritdoc />
        public override int Order => -Thousand;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlGenerator Generator { get; }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <inheritdoc />
        /// <remarks>Does nothing if <see cref="For"/> is <c>null</c>.</remarks>
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

            var tagBuilder = Generator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                labelText: null,
                htmlAttributes: null);

            if (For.ModelExplorer.Metadata.IsRequired)
            {
                // Specific for HRP Project
                tagBuilder.AddCssClass("text -has-required");
            }

            output.TagName = tagBuilder.TagName;
            output.MergeAttributes(tagBuilder);
            output.Content.SetHtmlContent(tagBuilder.InnerHtml);
        }
    }
}