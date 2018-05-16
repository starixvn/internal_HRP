namespace HRP.Core.UnitTests.CustomValidations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using HRP.Core.CustomValidations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class RequiredNoneOrAllAttributeTests
    {
        [Fact]
        public void IsValid_ValueIsNotNull_ReturnSuccess()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new EmployeePayrollTest
            {
                DirectBanking1CurrencyCode = "USD"
            };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void IsValid_RelatedPropertiesIsNull_ReturnSuccess()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new EmployeePayrollTest
            {
                DirectBanking1CurrencyCode = "USD"
            };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void IsValid_ValueIsNullAndRelatedPropertiesIsNotNull_ReturnError()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new EmployeePayrollTest
            {
                DirectBanking1Amount = "1"
            };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("Required", validationResults.First().ErrorMessage);
        }

        [Fact]
        public void AddValidation_WhenCalled_SetsAttributesAsExpected()
        {
            // Arrange
            var context = GetClientModelValidationContext();
            var attribute = new RequiredNoneOrAllAttribute
            {
                ErrorMessage = "Required"
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
                    Assert.Equal("data-val-requirednoneorall", kvp.Key);
                    Assert.Equal("Required", kvp.Value);
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

        private class EmployeePayrollTest
        {
            [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1Amount) }, ErrorMessage = "Required")]
            public string DirectBanking1CurrencyCode { get; set; }

            public string DirectBanking1Amount { get; set; }
        }
    }
}