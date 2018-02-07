namespace HRP.Core.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
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

    public class EmploymentChangeValueAttributeTests
    {
        #region IsValid

        [Fact]
        public void IsValid_ValueIsNullAndOldPropertyEqualNewProperty_ReturnSuccess()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new EmploymentTest
            {
                CompanyId = 1,
                OldCompanyId = 1
            };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void IsValid_ValueIsNullAndOldPropertyNotEqualNewProperty_ReturnError()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new EmploymentTest
            {
                CompanyId = 2,
                OldCompanyId = 1
            };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Single(validationResults);
        }

        [Fact]
        public void IsValid_ValueIsNotNull_ReturnSuccess()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new EmploymentTest
            {
                CompanyChangeDate = DateTime.Now
            };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Empty(validationResults);
        }

        #endregion IsValid

        #region AddValidation

        [Fact]
        public void AddValidation_ContextIsNull_ThrowException()
        {
            // Arrange
            var attribute = new EmploymentChangeValueAttribute();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => attribute.AddValidation(null));
            Assert.Equal("Value cannot be null.\r\nParameter name: context", ex.Message);
        }

        [Fact]
        public void AddValidation_ContextIsNotNull_SetsAttributesAsExpected()
        {
            // Arrange
            var context = GetClientModelValidationContext();
            var attribute = new EmploymentChangeValueAttribute
            {
                OldProperty = "OldProperty",
                NewProperty = "NewProperty",
                ErrorMessage = "Required"
            };

            // Act
            attribute.AddValidation(context);

            // Assert
            Assert.Collection(
                context.Attributes,
                kvp => { Assert.Equal("data-val", kvp.Key); Assert.Equal("true", kvp.Value); },
                kvp => { Assert.Equal("data-val-employmentchange", kvp.Key); Assert.Equal("Required", kvp.Value); },
                kvp => { Assert.Equal("data-val-newproperty", kvp.Key); Assert.Equal("NewProperty", kvp.Value); },
                kvp => { Assert.Equal("data-val-oldproperty", kvp.Key); Assert.Equal("OldProperty", kvp.Value); }
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

        private class EmploymentTest
        {
            public int CompanyId { get; set; }

            public int? OldCompanyId { get; set; }

            [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId), ErrorMessage = "Required")]
            public DateTime? CompanyChangeDate { get; set; }
        }
    }
}