// App.Contracts/Attributes/MinCountAttribute.cs
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace App.Contracts.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MinCountAttribute : ValidationAttribute
    {
        private readonly int _minCount;

        public MinCountAttribute(int minCount)
        {
            _minCount = minCount;
            ErrorMessage = $"حداقل {minCount} مورد باید انتخاب شود.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            if (value is IList list)
            {
                if (list.Count < _minCount)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, _minCount);
        }
    }
}