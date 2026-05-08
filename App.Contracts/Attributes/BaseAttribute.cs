// App.Contracts/Attributes/BaseAttribute.cs
using System.ComponentModel.DataAnnotations;

namespace App.Contracts.Attributes
{
    public abstract class BaseAttribute : ValidationAttribute
    {
        protected BaseAttribute(string defaultErrorMessage)
        {
            ErrorMessage = defaultErrorMessage;
        }

        protected string GetDisplayName(ValidationContext validationContext)
        {
            var displayName = validationContext.DisplayName;
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName ?? "");
            if (property != null)
            {
                var displayAttr = property.GetCustomAttributes(typeof(DisplayAttribute), false)
                    .FirstOrDefault() as DisplayAttribute;
                if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.Name))
                    displayName = displayAttr.Name;
            }
            return displayName;
        }
    }
}