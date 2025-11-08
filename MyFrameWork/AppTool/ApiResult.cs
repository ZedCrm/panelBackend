// MyFrameWork\AppTool\ApiResult.cs
using System;
using System.Collections.Generic;

namespace MyFrameWork.AppTool
{
    // نسخه غیرژنریک (برای پیام‌های ساده یا خطا)
    public class ApiResult
    {
        public bool IsSucceeded { get; set; } = true;
        public int StatusCode { get; set; } = 200;
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        // برای صفحه‌بندی
        public int? TotalRecords { get; set; }
        public int? TotalPages { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        // --- Factory Methods با دیفالت هوشمند ---
        public static ApiResult Success(
            string? message = null,
            int? statusCode = null)
        {
            return new ApiResult
            {
                IsSucceeded = true,
                Message = message ?? MessageApp.AcceptOpt,
                StatusCode = statusCode ?? 200
            };
        }

        public static ApiResult Failed(
            string? error = null,
            int? statusCode = null,
            List<string>? errors = null)
        {
            return new ApiResult
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

    // نسخه ژنریک — **همه جا از این استفاده می‌کنیم**
    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }

        // Success با داده
        public static ApiResult<T> Success(
            T data,
            string? message = null,
            int? statusCode = null)
        {
            return new ApiResult<T>
            {
                Data = data,
                IsSucceeded = true,
                Message = message ?? MessageApp.AcceptOpt,
                StatusCode = statusCode ?? 200
            };
        }

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

        // Failed — از پایه استفاده می‌کنه
        public new static ApiResult<T> Failed(
            string? error = null,
            int? statusCode = null,
            List<string>? errors = null)
        {
            return new ApiResult<T>
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
}