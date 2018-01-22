namespace HRP.Core.CustomValidations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredNoneOrAllAttribute : ValidationAttribute
    {
        public string[] RelatedProperties { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var properties = validationContext.ObjectType.GetProperties().Where(x => RelatedProperties.Any(r => r == x.Name));

            if (properties.Any(p => p.GetValue(validationContext.ObjectInstance, null) != null) && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}