namespace HRP.Core.UnitTests
{
    using System;
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
        public void AddValidation_ContextIsNull_ThrowException()
        {
            // Arrange
            var attribute = new RequiredIfRealtedFieldHasValueAttribute();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => attribute.AddValidation(null));
            Assert.Equal("Value cannot be null.\r\nParameter name: context", ex.Message);
        }

        [Fact]
        public void AddValidation_ContextIsNotNull_SetsAttributesAsExpected()
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
                kvp => { Assert.Equal("data-val", kvp.Key); Assert.Equal("true", kvp.Value); },
                kvp => { Assert.Equal("data-val-requiredifrelatedfieldhasvalue", kvp.Key); Assert.Equal("Required", kvp.Value); }
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
    }
}