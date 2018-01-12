namespace HRP.Core.CustomValidations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class EmploymentChangeValueAttribute : RequiredAttribute
    {
        public string OldProperty { get; set; }
        public string NewProperty { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var oldPropertyInfo = validationContext.ObjectType.GetProperty(OldProperty);
            var newPropertyInfo = validationContext.ObjectType.GetProperty(NewProperty);
            var oldValue = (IComparable)oldPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var newValue = (IComparable)newPropertyInfo.GetValue(validationContext.ObjectInstance, null);

#pragma warning disable S1067 // Expressions should not be too complex
            if (((oldValue != null && newValue == null)
                || (oldValue == null && newValue != null)
                || (oldValue != null && (oldValue.CompareTo(newValue) < 0 || oldValue.CompareTo(newValue) > 0))
                && value == null))
#pragma warning restore S1067 // Expressions should not be too complex
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}