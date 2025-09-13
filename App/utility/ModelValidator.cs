using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.utility
{
 public static class ModelValidator
    {
        public static OPT ValidateToOpt<T>(T model)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            if (results.Any())
            {
                return new OPT
                {
                    IsSucceeded = false,
                    Message = string.Join(" | ", results.Select(r => r.ErrorMessage))
                };
            }

            return new OPT { IsSucceeded = true }; // موفقیت بدون خطا
        }

        public static OPTResult<T> ValidateToOptResult<T>(T model) where T : class
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            if (results.Any())
            {
                return new OPTResult<T>
                {
                    IsSucceeded = false,
                    Message = string.Join(" | ", results.Select(r => r.ErrorMessage)),
                    Data = default
                };
            }

            return new OPTResult<T>
            {
                IsSucceeded = true,
               
               
            };
        }
    }
}