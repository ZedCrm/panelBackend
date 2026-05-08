// App.Contracts/Attributes/PersianTextAttribute.cs
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Contracts.Attributes
{
    /// <summary>
    /// فقط حروف فارسی و فاصله مجاز است
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PersianTextAttribute : ValidationAttribute
    {
        private static readonly Regex PersianRegex = new Regex(@"^[\u0600-\u06FF\s]+$", RegexOptions.Compiled);

        public PersianTextAttribute()
        {
            ErrorMessage = "فقط می‌توانید از حروف فارسی استفاده کنید.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value is not string stringValue)
                return ValidationResult.Success;

            if (string.IsNullOrWhiteSpace(stringValue))
                return ValidationResult.Success;

            if (!PersianRegex.IsMatch(stringValue))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}