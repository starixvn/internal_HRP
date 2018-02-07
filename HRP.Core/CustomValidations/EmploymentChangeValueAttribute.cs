namespace HRP.Core.CustomValidations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class EmploymentChangeValueAttribute : ValidationAttribute, IClientModelValidator
    {
        public string OldProperty { get; set; }
        public string NewProperty { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var oldPropertyInfo = validationContext.ObjectType.GetProperty(OldProperty);
            var newPropertyInfo = validationContext.ObjectType.GetProperty(NewProperty);
            var oldValue = (IComparable)oldPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var newValue = (IComparable)newPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (value == null && oldValue != null && (oldValue.CompareTo(newValue) < 0 || oldValue.CompareTo(newValue) > 0))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-employmentchange", ErrorMessage);
            MergeAttribute(context.Attributes, "data-val-oldproperty", OldProperty);
            MergeAttribute(context.Attributes, "data-val-newproperty", NewProperty);
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}