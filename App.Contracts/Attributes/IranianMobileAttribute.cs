// App.Contracts/Attributes/IranianMobileAttribute.cs
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Contracts.Attributes
{
    /// <summary>
    /// اعتبارسنجی شماره موبایل ایران
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IranianMobileAttribute : ValidationAttribute
    {
        private static readonly Regex MobileRegex = new Regex(@"^09[0-9]{9}$", RegexOptions.Compiled);

        public IranianMobileAttribute()
        {
            ErrorMessage = "شماره موبایل معتبر نیست. باید با 09 شروع شود و 11 رقم باشد.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value is not string stringValue)
                return ValidationResult.Success;

            if (string.IsNullOrWhiteSpace(stringValue))
                return ValidationResult.Success;

            if (!MobileRegex.IsMatch(stringValue))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}