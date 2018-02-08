namespace HRP.Core.UnitTests
{
    using System;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class GenericCompareAttributeTests
    {
        #region AddValidation

        [Fact]
        public void AddValidation_ContextIsNull_ThrowException()
        {
            // Arrange
            var attribute = new GenericCompareAttribute();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => attribute.AddValidation(null));
            Assert.Equal("Value cannot be null.\r\nParameter name: context", ex.Message);
        }

        [Theory]
        [InlineData(GenericCompareOperator.None, "")]
        [InlineData(GenericCompareOperator.GreaterThan, "Start Date must be greater than End Date.")]
        [InlineData(GenericCompareOperator.GreaterThanOrEqual, "Start Date must be greater than or equal to End Date.")]
        [InlineData(GenericCompareOperator.LessThan, "Start Date must be less than End Date.")]
        [InlineData(GenericCompareOperator.LessThanOrEqual, "Start Date must be less than or equal to End Date.")]
        public void AddValidation_ContextIsNotNull_SetsAttributesAsExpected(GenericCompareOperator oper, string errorMessage)
        {
            // Arrange
            var context = GetClientModelValidationContext();
            var attribute = new GenericCompareAttribute
            {
                DisplayName = "Start Date",
                CompareToPropertyName = "EndDate",
                OtherPropertyDisplayName = "End Date",
                Operator = oper,
                ErrorMessage = errorMessage
            };

            // Act
            attribute.AddValidation(context);

            // Assert
            Assert.Collection(
                context.Attributes,
                kvp => { Assert.Equal("data-val", kvp.Key); Assert.Equal("true", kvp.Value); },
                kvp => { Assert.Equal("data-val-comparetopropertyname", kvp.Key); Assert.Equal("EndDate", kvp.Value); },
                kvp => { Assert.Equal("data-val-genericcompare", kvp.Key); Assert.Equal(errorMessage, kvp.Value); },
                kvp => { Assert.Equal("data-val-operatorname", kvp.Key); Assert.Equal(oper.ToString(), kvp.Value); }
            );
        }

        #endregion AddValidation

        private static ClientModelValidationContext GetClientModelValidationContext()
        {
            IModelMetadataProvider metadataProvider = new EmptyModelMetadataProvider();
            var metadata = metadataProvider.GetMetadataForProperty(typeof(string), nameof(string.Length));
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            return new ClientModelValidationContext(
                 actionContext,
                 metadata,
                 metadataProvider,
                 new AttributeDictionary());
        }
    }
}