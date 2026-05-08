// App.Contracts/Attributes/RequiredIfAttribute.cs
using System.ComponentModel.DataAnnotations;

namespace App.Contracts.Attributes
{
    /// <summary>
    /// فیلد به صورت شرطی الزامی می‌شود
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _conditionProperty;
        private readonly object _conditionValue;

        public RequiredIfAttribute(string conditionProperty, object conditionValue)
        {
            _conditionProperty = conditionProperty;
            _conditionValue = conditionValue;
            ErrorMessage = "این فیلد الزامی است.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var conditionProperty = validationContext.ObjectType.GetProperty(_conditionProperty);
            if (conditionProperty == null)
                return new ValidationResult($"Property '{_conditionProperty}' not found.");

            var conditionValue = conditionProperty.GetValue(validationContext.ObjectInstance);

            if (Equals(conditionValue, _conditionValue))
            {
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

                if (value is int intValue && intValue <= 0)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}