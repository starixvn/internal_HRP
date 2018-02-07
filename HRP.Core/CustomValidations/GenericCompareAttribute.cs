namespace HRP.Core.CustomValidations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.Enumerations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GenericCompareAttribute : ValidationAttribute, IClientModelValidator
    {
        public string CompareToPropertyName { get; set; }
        public string DisplayName { get; set; }
        public string OtherPropertyDisplayName { get; set; }
        public GenericCompareOperator Operator { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var basePropertyInfo = validationContext.ObjectType.GetProperty(CompareToPropertyName);
            var valOther = (IComparable)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var valThis = (IComparable)value;

            if (IsGreaterThan(valThis, valOther) ||
                IsGreaterThanOrEqual(valThis, valOther) ||
                IsLessThan(valThis, valOther) ||
                IsLessThanOrEqual(valThis, valOther))
            {
                return new ValidationResult(GetErrorMessage());
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
            MergeAttribute(context.Attributes, "data-val-genericcompare", GetErrorMessage());
            MergeAttribute(context.Attributes, "data-val-comparetopropertyname", CompareToPropertyName);
            MergeAttribute(context.Attributes, "data-val-operatorname", Operator.ToString());
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }

        private bool IsGreaterThan(IComparable valThis, IComparable valOther)
        {
            return Operator == GenericCompareOperator.GreaterThan && valThis != null && valOther != null && valThis.CompareTo(valOther) <= 0;
        }

        private bool IsGreaterThanOrEqual(IComparable valThis, IComparable valOther)
        {
            return Operator == GenericCompareOperator.GreaterThanOrEqual && valThis != null && valOther != null && valThis.CompareTo(valOther) < 0;
        }

        private bool IsLessThan(IComparable valThis, IComparable valOther)
        {
            return Operator == GenericCompareOperator.LessThan && valThis != null && valOther != null && valThis.CompareTo(valOther) >= 0;
        }

        private bool IsLessThanOrEqual(IComparable valThis, IComparable valOther)
        {
            return Operator == GenericCompareOperator.LessThanOrEqual && valThis != null && valOther != null && valThis.CompareTo(valOther) > 0;
        }

        private string GetErrorMessage()
        {
            string errorMessage;

            switch (Operator)
            {
                case GenericCompareOperator.GreaterThan:
                    errorMessage = string.Format(Message.GreaterThanCompare, DisplayName, OtherPropertyDisplayName);
                    break;

                case GenericCompareOperator.GreaterThanOrEqual:
                    errorMessage = string.Format(Message.GreaterThanOrEqualCompare, DisplayName, OtherPropertyDisplayName);
                    break;

                case GenericCompareOperator.LessThan:
                    errorMessage = string.Format(Message.LessThanCompare, DisplayName, OtherPropertyDisplayName);
                    break;

                case GenericCompareOperator.LessThanOrEqual:
                    errorMessage = string.Format(Message.LessThanOrEqualCompare, DisplayName, OtherPropertyDisplayName);
                    break;

                default:
                    errorMessage = string.Empty;
                    break;
            }

            return errorMessage;
        }
    }
}