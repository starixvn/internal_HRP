namespace HRP.Core.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
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

        [Fact]
        public void AddValidation_ContextIsNull_ThrowException()
        {
            // Arrange
            var attribute = new EmploymentChangeValueAttribute();

            // Act
            Action testDelegate = () => attribute.AddValidation(null);

            // Assert
            Assert.Throws<ArgumentNullException>(testDelegate);
        }
    }

    public class EmploymentTest
    {
        public int CompanyId { get; set; }

        public int? OldCompanyId { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId), ErrorMessage = "Required")]
        public DateTime? CompanyChangeDate { get; set; }
    }
}