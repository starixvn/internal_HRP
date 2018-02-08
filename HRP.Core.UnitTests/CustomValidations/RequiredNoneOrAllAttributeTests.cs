namespace HRP.Core.UnitTests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using HRP.Core.CustomValidations;
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

        private class EmployeePayrollTest
        {
            [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1Amount), nameof(DirectBanking1AccountId), nameof(DirectBanking1BankTypeId) },
                ErrorMessage = "Required")]
            public string DirectBanking1CurrencyCode { get; set; }

            public string DirectBanking1Amount { get; set; }

            public int? DirectBanking1AccountId { get; set; }

            public int? DirectBanking1BankTypeId { get; set; }
        }
    }
}