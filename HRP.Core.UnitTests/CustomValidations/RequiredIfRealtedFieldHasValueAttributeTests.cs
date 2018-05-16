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

    public class RequiredIfRealtedFieldHasValueAttributeTests
    {
        [Fact]
        public void IsValid_ValueIsNotNull_ReturnSuccess()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new AdditionModelTest
            {
                UnitAmount = "1000"
            };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void IsValid_RelatedPropertyIsNull_ReturnSuccess()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new AdditionModelTest
            {
                UnitAmount = "1000"
            };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void IsValid_ValueIsNullAndRelatedPropertyIsNotNull_ReturnError()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new AdditionModelTest
            {
                NoOfDays = 1
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
            var attribute = new RequiredIfRealtedFieldHasValueAttribute
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
                    Assert.Equal("data-val-requiredifrelatedfieldhasvalue", kvp.Key);
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

        private class AdditionModelTest
        {
            [RequiredIfRealtedFieldHasValue(RelatedName = nameof(NoOfDays), ErrorMessage = "Required")]
            public string UnitAmount { get; set; }

            public decimal? NoOfDays { get; set; }
        }
    }
}