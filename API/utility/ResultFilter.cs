using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyFrameWork.AppTool.ResultType;

namespace API.utility
{
    public class ResultFilter : IActionFilter
    {
        // این متد BEFORE اجرای اکشن صدا زده می‌شود
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // کاری نداریم - فقط بگذار خالی باشد
            // (در آینده می‌توانی Validation خودکار اینجا بگذاری)
        }

        // این متد AFTER اجرای اکشن صدا زده می‌شود
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // اگر نتیجه از نوع ObjectResult باشد و مقدار آن StatusResult باشد
            if (context.Result is ObjectResult objResult && objResult.Value is StatusResult statusResult)
            {
                // ساختار خروجی یکسان برای همه API‌ها
                var response = new
                {
                    success = statusResult.IsSuccess,
                    message = statusResult.Messages?.FirstOrDefault(),
                    errors = statusResult.IsSuccess ? null : statusResult.Messages,
                    data = GetData(objResult.Value),
                    pagination = GetPagination(objResult.Value)
                };

                context.Result = new OkObjectResult(response);
            }
        }

        private object? GetData(object? value)
        {
            if (value == null) return null;

            // اگر SingleDataResult باشد
            var singleType = value.GetType();
            var singleDataProp = singleType.GetProperty("SingleData");
            if (singleDataProp != null)
                return singleDataProp.GetValue(value);

            // اگر ListDataResult باشد
            var dataProp = singleType.GetProperty("Data");
            if (dataProp != null)
                return dataProp.GetValue(value);

            return null;
        }

        private object? GetPagination(object? value)
        {
            if (value == null) return null;

            var type = value.GetType();
            var pageNumberProp = type.GetProperty("PageNumber");
            var pageSizeProp = type.GetProperty("PageSize");
            var totalRecordsProp = type.GetProperty("TotalRecords");

            if (pageNumberProp != null && pageSizeProp != null)
            {
                return new
                {
                    totalRecords = totalRecordsProp?.GetValue(value),
                    pageNumber = pageNumberProp.GetValue(value),
                    pageSize = pageSizeProp.GetValue(value)
                };
            }

            return null;
        }
    }
}