using ConfApp;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.utility
{
    public static class ValidationUtility
    {
        /// <summary>
        /// اعتبارسنجی خالی نبودن مقدار
        /// </summary>
        public static StatusResult ValidateNotEmpty(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} نمی‌تواند خالی باشد."
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی طول رشته
        /// </summary>
        public static StatusResult ValidateLength(string value, string fieldName, int minLength, int maxLength)
        {
            if (value == null)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} نمی‌تواند خالی باشد."
                );
            }

            if (value.Length < minLength || value.Length > maxLength)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} باید بین {minLength} تا {maxLength} کاراکتر باشد."
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی محدوده عددی
        /// </summary>
        public static StatusResult ValidateRange(decimal value, string fieldName, decimal minValue, decimal maxValue)
        {
            if (value < minValue || value > maxValue)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} باید بین {minValue} تا {maxValue} باشد."
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی محدوده عددی برای int
        /// </summary>
        public static StatusResult ValidateRange(int value, string fieldName, int minValue, int maxValue)
        {
            if (value < minValue || value > maxValue)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} باید بین {minValue} تا {maxValue} باشد."
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی محدوده عددی برای double
        /// </summary>
        public static StatusResult ValidateRange(double value, string fieldName, double minValue, double maxValue)
        {
            if (value < minValue || value > maxValue)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} باید بین {minValue} تا {maxValue} باشد."
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی یکتا بودن مقدار در دیتابیس با استفاده از MyContext
        /// </summary>
        public static async Task<StatusResult> ValidateUniqueAsync<T>(
            MyContext context,
            Expression<Func<T, bool>> predicate,
            string errorMessage) where T : class
        {
            if (context == null)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    "Context معتبر نیست."
                );
            }

            var exists = await context.Set<T>().AnyAsync(predicate);
            if (exists)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    errorMessage
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی یکتا بودن با امکان ارسال پیام پیش‌فرض
        /// </summary>
        public static async Task<StatusResult> ValidateUniqueAsync<T>(
            MyContext context,
            Expression<Func<T, bool>> predicate,
            string fieldName,
            string value) where T : class
        {
            var errorMessage = $"{fieldName} با مقدار '{value}' قبلاً در سیستم ثبت شده است.";
            return await ValidateUniqueAsync(context, predicate, errorMessage);
        }

        /// <summary>
        /// اعتبارسنجی یکتا بودن برای ویرایش (به جز آیتم جاری)
        /// </summary>
        public static async Task<StatusResult> ValidateUniqueForUpdateAsync<T>(
            MyContext context,
            Expression<Func<T, bool>> predicate,
            int currentId,
            string fieldName,
            string value) where T : Domain.Objects.BaseDomain
        {
            // شرط: آیتم با همین مقدار وجود داشته باشد اما Id آن برابر با currentId نباشد
            var combinedPredicate = predicate.And(x => x.Id != currentId);
            var errorMessage = $"{fieldName} با مقدار '{value}' قبلاً در سیستم ثبت شده است.";

            var exists = await context.Set<T>().AnyAsync(combinedPredicate);
            if (exists)
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    errorMessage
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی الگوی ایمیل
        /// </summary>
        public static StatusResult ValidateEmail(string email, string fieldName = "ایمیل")
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} نمی‌تواند خالی باشد."
                );
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    return ResultFactory.Status(
                        ResultStatusEnum.ValidationFailed,
                        $"{fieldName} معتبر نمی‌باشد."
                    );
                }
            }
            catch
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} معتبر نمی‌باشد."
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی الگوی تلفن همراه ایران
        /// </summary>
        public static StatusResult ValidateMobilePhone(string mobile, string fieldName = "تلفن همراه")
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} نمی‌تواند خالی باشد."
                );
            }

            // الگوی ساده برای شماره موبایل ایران
            var mobilePattern = @"^09[0-9]{9}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(mobile, mobilePattern))
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} معتبر نمی‌باشد. شماره باید با 09 شروع شده و 11 رقم باشد."
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی الگوی کد ملی ایران
        /// </summary>
        public static StatusResult ValidateNationalCode(string nationalCode, string fieldName = "کد ملی")
        {
            if (string.IsNullOrWhiteSpace(nationalCode))
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} نمی‌تواند خالی باشد."
                );
            }

            // الگوی ساده برای کد ملی
            var nationalCodePattern = @"^\d{10}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(nationalCode, nationalCodePattern))
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    $"{fieldName} باید 10 رقم باشد."
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// اعتبارسنجی چندگانه (ترکیب چند اعتبارسنجی)
        /// </summary>
        public static StatusResult ValidateAll(params StatusResult[] validationResults)
        {
            var errors = new List<string>();

            foreach (var result in validationResults)
            {
                if (!result.IsSuccess && result.Messages != null)
                {
                    errors.AddRange(result.Messages);
                }
            }

            if (errors.Any())
            {
                return ResultFactory.Status(
                    ResultStatusEnum.ValidationFailed,
                    errors.ToArray()
                );
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        /// <summary>
        /// بررسی می‌کند که آیا همه اعتبارسنجی‌ها موفق بوده‌اند
        /// </summary>
        public static bool IsAllValid(params StatusResult[] validationResults)
        {
            return validationResults.All(r => r.IsSuccess);
        }

        /// <summary>
        /// دریافت لیست خطاها از چندین نتیجه اعتبارسنجی
        /// </summary>
        public static List<string> GetErrors(params StatusResult[] validationResults)
        {
            var errors = new List<string>();

            foreach (var result in validationResults)
            {
                if (!result.IsSuccess && result.Messages != null)
                {
                    errors.AddRange(result.Messages);
                }
            }

            return errors;
        }
    }
}