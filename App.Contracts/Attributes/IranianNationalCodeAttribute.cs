// App.Contracts/Attributes/IranianNationalCodeAttribute.cs
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Contracts.Attributes
{
    /// <summary>
    /// اعتبارسنجی کد ملی ایران
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IranianNationalCodeAttribute : ValidationAttribute
    {
        private static readonly Regex NationalCodeRegex = new Regex(@"^\d{10}$", RegexOptions.Compiled);

        public IranianNationalCodeAttribute()
        {
            ErrorMessage = "کد ملی معتبر نیست. باید 10 رقم باشد.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value is not string stringValue)
                return ValidationResult.Success;

            if (string.IsNullOrWhiteSpace(stringValue))
                return ValidationResult.Success;

            if (!NationalCodeRegex.IsMatch(stringValue))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            // الگوریتم کنترلی کد ملی
            if (!IsValidNationalCode(stringValue))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }

        private static bool IsValidNationalCode(string code)
        {
            if (code.Length != 10)
                return false;

            var allDigitSame = code.All(c => c == code[0]);
            if (allDigitSame)
                return false;

            var check = int.Parse(code[9].ToString());
            var sum = Enumerable.Range(0, 9)
                .Select(i => int.Parse(code[i].ToString()) * (10 - i))
                .Sum();

            var remainder = sum % 11;
            return remainder < 2 && check == remainder || remainder >= 2 && check == 11 - remainder;
        }
    }
}