// MyFrameWork/AppTool/ApiResult.cs
using System;
using System.Collections.Generic;

namespace MyFrameWork.AppTool
{
    // نسخه پایه — برای عملیات بدون دیتا (مثل MarkAsRead)
    public class ApiResult
    {
        public bool IsSucceeded { get; set; } = true;
        public int StatusCode { get; set; } = 200;
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        // صفحه‌بندی
        public int? TotalRecords { get; set; }
        public int? TotalPages { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        // --- Factory Methods ---
        public static ApiResult Success(string? message = null, int? statusCode = null)
            => new()
            {
                IsSucceeded = true,
                Message = message ?? MessageApp.AcceptOpt,
                StatusCode = statusCode ?? 200
            };

        public static ApiResult Failed(string? error = null, int? statusCode = null, List<string>? errors = null)
            => new()
            {
                IsSucceeded = false,
                Message = error ?? MessageApp.FailOpt,
                StatusCode = statusCode ?? 400,
                Errors = errors?.Count > 0
                    ? errors
                    : (string.IsNullOrWhiteSpace(error)
                        ? new List<string> { MessageApp.FailOpt }
                        : new List<string> { error })
            };
    }

    // نسخه ژنریک — برای همه پاسخ‌های دارای دیتا
    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }

        // Success با دیتا
        public static ApiResult<T> Success(T data, string? message = null, int? statusCode = null)
            => new()
            {
                Data = data,
                IsSucceeded = true,
                Message = message ?? MessageApp.AcceptOpt,
                StatusCode = statusCode ?? 200
            };


        // Success با دیتا
        public static ApiResult<List<T>> Success(List<T> data, string? message = null, int? statusCode = null)
            => new()
            {
                Data = data,
                IsSucceeded = true,
                Message = message ?? MessageApp.AcceptOpt,
                StatusCode = statusCode ?? 200
            };
        // Paged Success
        public static ApiResult<T> PagedSuccess(
            T data,
            int totalRecords,
            int pageNumber,
            int pageSize,
            string? message = null,
            int? statusCode = null)
        {
            var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalRecords / (double)pageSize) : 0;

            return new ApiResult<T>
            {
                Data = data,
                IsSucceeded = true,
                Message = message ?? MessageApp.AcceptOpt,
                StatusCode = statusCode ?? 200,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Failed
        public new static ApiResult<T> Failed(string? error = null, int? statusCode = null, List<string>? errors = null)
            => new()
            {
                IsSucceeded = false,
                Message = error ?? MessageApp.FailOpt,
                StatusCode = statusCode ?? 400,
                Errors = errors?.Count > 0
                    ? errors
                    : (string.IsNullOrWhiteSpace(error)
                        ? new List<string> { MessageApp.FailOpt }
                        : new List<string> { error })
            };
    }




    
}