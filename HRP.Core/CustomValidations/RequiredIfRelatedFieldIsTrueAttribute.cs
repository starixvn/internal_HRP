namespace HRP.Core.CustomValidations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfRelatedFieldIsTrueAttribute : ValidationAttribute, IClientModelValidator
    {
        public string RelatedFieldName { get; set; }

        public object DesiredValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;

            Type type = instance.GetType();
            object propertyvalue = type.GetProperty(RelatedFieldName)?.GetValue(instance, null);

            if (propertyvalue?.ToString() == DesiredValue.ToString() && value == null)
            {
                return new ValidationResult(GetErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-requiredifrelatedfieldistrue", GetErrorMessage(context.ModelMetadata.DisplayName));
            MergeAttribute(context.Attributes, "data-val-requiredifrelatedfieldistrue-relatedfieldname", RelatedFieldName);
            MergeAttribute(context.Attributes, "data-val-requiredifrelatedfieldistrue-desiredvalue", DesiredValue.ToString());
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }

        private string GetErrorMessage(string displayName)
        {
            return string.Format(ErrorMessageString, displayName);
        }
    }
}
