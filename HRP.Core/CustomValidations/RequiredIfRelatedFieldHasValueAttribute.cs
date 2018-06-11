namespace HRP.Core.CustomValidations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class RequiredIfRelatedFieldHasValueAttribute : ValidationAttribute, IClientModelValidator
    {
        public string RelatedName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var relatedInfo = validationContext.ObjectType.GetProperty(RelatedName);

            if (relatedInfo.GetValue(validationContext.ObjectInstance, null) != null && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-requiredifrelatedfieldhasvalue", ErrorMessageString);
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
