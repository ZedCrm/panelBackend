using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFrameWork.AppTool
{
    public static class Validator
    {

        public static void CheckRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"مقدار {fieldName} اجباریست.");
            }
        }

        public static void CheckStringLength(string value, int minLength, int maxLength, string fieldName)
        {
            if (value.Length < minLength || value.Length > maxLength)
            {
                throw new ArgumentException($"مقدار کارکتر {fieldName} باید بین {minLength} و {maxLength} . باشد ");
            }
        }

       
    }
}
