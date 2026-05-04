using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyFrameWork.AppTool.ResultType;

namespace App.utility
{
    public static class ModelValidator
    {
        /// <summary>
        /// اعتبارسنجی مدل و برگرداندن نتیجه به صورت StatusResult
        /// </summary>
        public static StatusResult ValidateToStatusResult<T>(T model)
        {
            if (model == null)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    "Model cannot be null"
                );
            }

            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                model, 
                context, 
                results, 
                validateAllProperties: true
            );

            if (!isValid && results.Any())
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    results.Select(r => r.ErrorMessage).ToArray()
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی مدل و برگرداندن نتیجه به صورت SingleDataResult
        /// در صورت موفقیت، خود مدل برگردانده می‌شود
        /// </summary>
        public static SingleDataResult<T> ValidateToSingleResult<T>(T model)
        {
            if (model == null)
            {
                return ResultFactory.Single<T>(
                    ResultStatusEnum.ValidationFailed,
                    default,
                    "Model cannot be null"
                );
            }

            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                model, 
                context, 
                results, 
                validateAllProperties: true
            );

            if (!isValid && results.Any())
            {
                return ResultFactory.Single<T>(
                    ResultStatusEnum.ValidationFailed,
                    default,
                    results.Select(r => r.ErrorMessage).ToArray()
                );
            }

            return ResultFactory.Single(
                ResultStatusEnum.Success,
                model
            );
        }

        /// <summary>
        /// اعتبارسنجی مدل و برگرداندن نتیجه به صورت دلخواه با امکان ارسال دیتای سفارشی
        /// </summary>
        public static SingleDataResult<TOutput> ValidateAndMap<TInput, TOutput>(
            TInput model, 
            Func<TInput, TOutput> onSuccessMapping)
        {
            if (model == null)
            {
                return ResultFactory.Single<TOutput>(
                    ResultStatusEnum.ValidationFailed,
                    default,
                    "Model cannot be null"
                );
            }

            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                model, 
                context, 
                results, 
                validateAllProperties: true
            );

            if (!isValid && results.Any())
            {
                return ResultFactory.Single<TOutput>(
                    ResultStatusEnum.ValidationFailed,
                    default,
                    results.Select(r => r.ErrorMessage).ToArray()
                );
            }

            var output = onSuccessMapping(model);
            return ResultFactory.Single(
                ResultStatusEnum.Success,
                output
            );
        }

        /// <summary>
        /// اعتبارسنجی آسنکرون مدل
        /// </summary>
        public static async Task<StatusResult> ValidateToStatusResultAsync<T>(
            T model, 
            Func<T, Task<bool>>? additionalValidation = null)
        {
            if (model == null)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    "Model cannot be null"
                );
            }

            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                model, 
                context, 
                results, 
                validateAllProperties: true
            );

            if (!isValid && results.Any())
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    results.Select(r => r.ErrorMessage).ToArray()
                );
            }

            // اعتبارسنجی اضافی توسط کالر
            if (additionalValidation != null)
            {
                var isAdditionalValid = await additionalValidation(model);
                if (!isAdditionalValid)
                {
                    return ResultFactory.Status(
                        ResultStatusEnum.ValidationFailed,
                        "Additional validation failed"
                    );
                }
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// بررسی می‌کند که آیا مدل معتبر است یا خیر (بدون ایجاد آبجکت نتیجه)
        /// </summary>
        public static bool IsValid<T>(T model, out List<string> errors)
        {
            errors = new List<string>();

            if (model == null)
            {
                errors.Add("Model cannot be null");
                return false;
            }

            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                model, 
                context, 
                results, 
                validateAllProperties: true
            );

            if (!isValid && results.Any())
            {
                errors.AddRange(results.Select(r => r.ErrorMessage));
                return false;
            }

            return true;
        }
    }
}