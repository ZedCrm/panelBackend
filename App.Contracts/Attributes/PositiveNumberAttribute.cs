// App.Contracts/Attributes/PositiveNumberAttribute.cs
using System.ComponentModel.DataAnnotations;

namespace App.Contracts.Attributes
{
    /// <summary>
    /// عدد باید مثبت باشد
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PositiveNumberAttribute : ValidationAttribute
    {
        public PositiveNumberAttribute()
        {
            ErrorMessage = "مقدار باید بزرگتر از صفر باشد.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (value is int intValue && intValue <= 0)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            if (value is decimal decimalValue && decimalValue <= 0)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            if (value is double doubleValue && doubleValue <= 0)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}