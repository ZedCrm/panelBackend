using System;
using System.Collections.Generic;

namespace MyFrameWork.AppTool
{
    public class OPTResult<T> where T : class
    {
        public bool? IsSucceeded { get; set; }   // وضعیت موفقیت عملیات
        public string? Message { get; set; }     // پیام عملیات
        public List<T>? Data { get; set; }       // لیست داده‌ها برای حالت لیستی
        public T? SingleData { get; set; }       // داده‌ی تکی برای عملیات‌های خاص
        public int? TotalRecords { get; set; }   // مجموع رکوردها (در صفحه‌بندی)
        public int? TotalPages { get; set; }     // مجموع صفحات (در صفحه‌بندی)
        public int? PageNumber { get; set; }     // شماره صفحه جاری
        public int? PageSize { get; set; }       // تعداد آیتم در هر صفحه

        // متد سازنده پیش‌فرض
        public OPTResult()
        {
            IsSucceeded = true;
        }

        // ساخت نتیجه موفق برای لیست داده‌ها
        public static OPTResult<T> Success(List<T> data, string message = "عملیات با موفقیت انجام شد")
        {
            return new OPTResult<T>
            {
                IsSucceeded = true,
                Message = message,
                Data = data
            };
        }

        // ساخت نتیجه موفق برای یک داده تکی
        public static OPTResult<T> Success(T singleData, string message = "عملیات با موفقیت انجام شد")
        {
            return new OPTResult<T>
            {
                IsSucceeded = true,
                Message = message,
                SingleData = singleData
            };
        }

        // ساخت نتیجه ناموفق با پیام خطا
        public static OPTResult<T> Failed(string message = "عملیات با شکست مواجه شد")
        {
            return new OPTResult<T>
            {
                IsSucceeded = false,
                Message = message
            };
        }
    }
}
