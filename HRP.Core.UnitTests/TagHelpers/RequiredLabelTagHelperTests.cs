namespace HRP.Core.UnitTests.TagHelpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using HRP.Core.Extensions.TagHelpers;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using Xunit;

    public class RequiredLabelTagHelperTests
    {
        [Fact]
        public void Process_TagHelperContextIsNull_ThrowException()
        {
            // Arrange
            IModelMetadataProvider metadataProvider = new EmptyModelMetadataProvider();
            var htmlGenerator = new MockHtmlGenerator(metadataProvider);
            var tagHelper = new RequiredLabelTagHelper(htmlGenerator);

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => tagHelper.Process(null, null));
            Assert.Equal("Value cannot be null.\r\nParameter name: context", ex.Message);
        }

        [Fact]
        public void Process_TagHelperOutputIsNull_ThrowException()
        {
            // Arrange
            IModelMetadataProvider metadataProvider = new EmptyModelMetadataProvider();
            var htmlGenerator = new MockHtmlGenerator(metadataProvider);
            var tagHelperContext = new TagHelperContext(
                tagName: "not-label",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");
            var tagHelper = new RequiredLabelTagHelper(htmlGenerator);

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => tagHelper.Process(tagHelperContext, null));
            Assert.Equal("Value cannot be null.\r\nParameter name: output", ex.Message);
        }

        [Fact]
        public void Process_WhenCalled_GeneratesExpectedOutput()
        {
            // Arrange
            var expectedTagName = "label";
            var expectedPreContent = "original pre-content";
            var expectedPostContent = "original post-content";
            IModelMetadataProvider metadataProvider = new EmptyModelMetadataProvider();
            var htmlGenerator = new MockHtmlGenerator(metadataProvider);
            var tagHelperContext = new TagHelperContext(
                tagName: "label",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");
            var htmlAttributes = new TagHelperAttributeList
            {
                { "class", "form-control" }
            };
            var output = new TagHelperOutput(
                expectedTagName,
                htmlAttributes,
                getChildContentAsync: (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.AppendHtml("Hello World");
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });
            output.PreContent.AppendHtml(expectedPreContent);
            output.PostContent.AppendHtml(expectedPostContent);

            var model = new Model { Text = "inner text" };
            var containerExplorer = metadataProvider.GetModelExplorerForType(typeof(Model), model);
            var propertyMetadata = metadataProvider.GetMetadataForProperty(typeof(Model), "Text");
            object modelAccessor() => model.Text;
            var modelExplorer = containerExplorer.GetExplorerForExpression(propertyMetadata, modelAccessor());
            var modelExpression = new ModelExpression("[0].Model.Text", modelExplorer);

            var tagHelper = new RequiredLabelTagHelper(htmlGenerator)
            {
                For = modelExpression
            };

            var viewContext = MockHtmlGenerator.GetViewContext(model, htmlGenerator, metadataProvider);
            tagHelper.ViewContext = viewContext;

            // Act
            tagHelper.Process(tagHelperContext, output);

            // Assert
            Assert.Equal(expectedPreContent, output.PreContent.GetContent());
            Assert.Equal(expectedPostContent, output.PostContent.GetContent());
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.Equal(expectedTagName, output.TagName);
        }

        private class Model
        {
            [Required]
            public string Text { get; set; }
        }
    }
}