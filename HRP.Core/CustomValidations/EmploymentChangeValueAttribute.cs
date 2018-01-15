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

            if (oldValue != null && (oldValue.CompareTo(newValue) < 0 || oldValue.CompareTo(newValue) > 0) && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}