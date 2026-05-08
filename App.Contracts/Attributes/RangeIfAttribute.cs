// App.Contracts/Attributes/RangeIfAttribute.cs
using System.ComponentModel.DataAnnotations;

namespace App.Contracts.Attributes
{
    /// <summary>
    /// بررسی محدوده مقدار
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RangeIfAttribute : ValidationAttribute
    {
        private readonly int _min;
        private readonly int _max;

        public RangeIfAttribute(int min, int max)
        {
            _min = min;
            _max = max;
            ErrorMessage = $"مقدار باید بین {min} و {max} باشد.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (value is int intValue)
            {
                if (intValue < _min || intValue > _max)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else if (value is decimal decimalValue)
            {
                if (decimalValue < _min || decimalValue > _max)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else if (value is double doubleValue)
            {
                if (doubleValue < _min || doubleValue > _max)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}