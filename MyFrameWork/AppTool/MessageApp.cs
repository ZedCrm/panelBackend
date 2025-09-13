using System;

namespace MyFrameWork.AppTool
{
    public static class MessageApp
    {
        // ------------------------------
        // پیام‌های ثابت عمومی
        // ------------------------------
        public const string AcceptOpt = "عملیات با موفقیت انجام شد.";
        public const string FailOpt = "عملیات با خطا مواجه شد!!!";
        public const string NotPermission = "شما دسترسی لازم را ندارید.";
        public const string OperationCancelled = "عملیات لغو شد.";

        // پیام‌های خطا
        public const string ServerError = "خطای سرور رخ داده است. لطفاً بعداً تلاش کنید.";
        public const string ConnectionFailed = "اتصال به سرور برقرار نشد.";
        public const string Timeout = "زمان درخواست به پایان رسید.";
        public const string InvalidFormat = "فرمت ورودی صحیح نیست.";
        public const string InvalidInput = "ورودی وارد شده معتبر نیست.";

        // پیام‌های اعتبارسنجی
        public const string IsRequired = "این مقدار نمی‌تواند خالی باشد.";
        public const string MaxLength = "تعداد کاراکتر وارد شده بیشتر از حد مجاز است.";
        public const string MinLength = "تعداد کاراکتر وارد شده کمتر از حد مجاز است.";
        public const string NotFound = "مورد مورد نظر یافت نشد.";
        public const string DuplicateEntry = "این مقدار قبلاً ثبت شده است.";

        // ------------------------------
        // متدهای پویا برای پیام‌های قابل شخصی‌سازی
        // ------------------------------
        
        public static string RequiredField(string fieldName)
            => $"{fieldName} نمی‌تواند خالی باشد.";

        public static string MaxLengthExceeded(string fieldName, int maxLength)
            => $"تعداد کاراکترهای {fieldName} بیشتر از {maxLength} کاراکتر است.";

        public static string MinLengthNotMet(string fieldName, int minLength)
            => $"تعداد کاراکترهای {fieldName} کمتر از {minLength} کاراکتر است.";

        public static string NotFoundItem(string itemName)
            => $"{itemName} یافت نشد.";

        public static string DuplicateField(string fieldName)
            => $"{fieldName} قبلاً ثبت شده است.";

        public static string InvalidFieldInput(string fieldName)
            => $"مقدار وارد شده برای {fieldName} معتبر نیست.";

        public static string CustomError(string errorDetail)
            => $"خطا: {errorDetail}";

        public static string CustomSuccess(string action)
            => $"عملیات {action} با موفقیت انجام شد.";

        public static string CustomFail(string action)
            => $"عملیات {action} با خطا مواجه شد: {action}";
        

        public static string CustomDeleteFail(string action)
            => $". حذف با خطا مواجه شد: {action}";
        public static string CustomAddsuccses(string action)
            => $". رکورد با موفقیت ذخیره  شد: {action}";

        // ------------------------------
        // پیام‌های اطلاع‌رسانی
        // ------------------------------
        public const string InfoNoChanges = "هیچ تغییر جدیدی اعمال نشد.";
        public const string InfoSaved = "اطلاعات با موفقیت ذخیره شد.";
        public const string InfoDeleted = "مورد با موفقیت حذف شد.";
    
                // ------------------------------
        // پیام‌های اعتبارسنجی (DataAnnotation)
        // ------------------------------
        public const string IsRequiredcustom = "لطفاً {0} را وارد کنید.";
        public const string MaxLengthcustom = "تعداد کاراکترهای {0} نمی‌تواند بیشتر از {1} باشد.";
        public const string MinLengthcustom = "تعداد کاراکترهای {0} نمی‌تواند کمتر از {1} باشد.";
        public const string NotFoundcustom = "{0} یافت نشد.";
        public const string DuplicateEntrycustom = "{0} قبلاً ثبت شده است.";
        public const string InvalidInputcustom = "مقدار وارد شده برای {0} معتبر نیست.";
    
    
    
    
    
    }
}
