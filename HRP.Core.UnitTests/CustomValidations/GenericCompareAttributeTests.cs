namespace HRP.Core.UnitTests.CustomValidations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
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
        [Fact]
        public void IsValid_ValueIsNotNull_ReturnSuccess()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new EmploymentTest { NewValue = 1 };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void IsValid_NewValueIsLessThanOldValue_ReturnError()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new EmploymentTest { OldValue = 2 , NewValue = 1 };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("Old value must be greater than New value.", validationResults.First().ErrorMessage);
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
                kvp =>
                {
                    Assert.Equal("data-val", kvp.Key);
                    Assert.Equal("true", kvp.Value);
                },
                kvp =>
                {
                    Assert.Equal("data-val-comparetopropertyname", kvp.Key);
                    Assert.Equal("EndDate", kvp.Value);
                },
                kvp =>
                {
                    Assert.Equal("data-val-genericcompare", kvp.Key);
                    Assert.Equal(errorMessage, kvp.Value);
                },
                kvp =>
                {
                    Assert.Equal("data-val-operatorname", kvp.Key);
                    Assert.Equal(oper.ToString(), kvp.Value);
                }
            );
        }

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

        private class EmploymentTest
        {
            public int? OldValue { get; set; }

            [GenericCompare(
                CompareToPropertyName = nameof(OldValue),
                DisplayName = "Old value",
                OtherPropertyDisplayName = "New value",
                Operator = GenericCompareOperator.GreaterThan)]
            public int NewValue { get; set; }
        }
    }
}