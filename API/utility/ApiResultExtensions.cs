using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool.ResultType;
using FileResult = MyFrameWork.AppTool.ResultType.FileResult;

namespace API.utility
{
    public static class ApiResultExtensions
    {
        /// <summary>
        /// تبدیل StatusResult به ActionResult
        /// </summary>
        public static IActionResult ToActionResult(this StatusResult result)
        {
            if (result == null) 
                return new StatusCodeResult(500);

            var statusCode = GetHttpStatusCode(result.Status);
            
            var content = new
            {
                success = result.IsSuccess,
                message = result.Messages?.FirstOrDefault(),
                errors = !result.IsSuccess ? result.Messages : null,
                data = (object?)null,
                pagination = (object?)null
            };

            return statusCode >= 200 && statusCode < 300
                ? new OkObjectResult(content) { StatusCode = statusCode }
                : new ObjectResult(content) { StatusCode = statusCode };
        }

        /// <summary>
        /// تبدیل SingleDataResult به ActionResult
        /// </summary>
        public static IActionResult ToActionResult<T>(this SingleDataResult<T> result)
        {
            if (result == null) 
                return new StatusCodeResult(500);

            var statusCode = GetHttpStatusCode(result.Status);
            
            var content = new
            {
                success = result.IsSuccess,
                message = result.Messages?.FirstOrDefault(),
                errors = !result.IsSuccess ? result.Messages : null,
                data = result.SingleData,
                pagination = (object?)null
            };

            return statusCode >= 200 && statusCode < 300
                ? new OkObjectResult(content) { StatusCode = statusCode }
                : new ObjectResult(content) { StatusCode = statusCode };
        }

        /// <summary>
        /// تبدیل ListDataResult به ActionResult
        /// </summary>
        public static IActionResult ToActionResult<T>(this ListDataResult<T> result) where T : class
        {
            if (result == null) 
                return new StatusCodeResult(500);

            var statusCode = GetHttpStatusCode(result.Status);
            
            var content = new
            {
                success = result.IsSuccess,
                message = result.Messages?.FirstOrDefault(),
                errors = !result.IsSuccess ? result.Messages : null,
                data = result.Data,
                pagination = new
                {
                    totalRecords = result.TotalRecords,
                    pageNumber = result.PageNumber,
                    pageSize = result.PageSize
                }
            };

            return statusCode >= 200 && statusCode < 300
                ? new OkObjectResult(content) { StatusCode = statusCode }
                : new ObjectResult(content) { StatusCode = statusCode };
        }

        /// <summary>
        /// تبدیل FileResult به ActionResult
        /// </summary>
        public static IActionResult ToActionResult(this FileResult result)
        {
            if (result == null) 
                return new StatusCodeResult(500);

            if (!result.IsSuccess)
            {
                var statusCode = GetHttpStatusCode(result.Status);
                var content = new
                {
                    success = false,
                    message = result.Messages?.FirstOrDefault(),
                    errors = result.Messages,
                    data = (object?)null
                };
                return new ObjectResult(content) { StatusCode = statusCode };
            }

            // در صورت موفقیت، فایل برگردانده می‌شود
            if (!string.IsNullOrEmpty(result.FilePath) && System.IO.File.Exists(result.FilePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(result.FilePath);
                return new FileContentResult(fileBytes, "application/octet-stream");
            }

            return new NotFoundObjectResult(new { success = false, message = "فایل یافت نشد" });
        }

        /// <summary>
        /// متد کمکی برای تبدیل ResultStatusEnum به HttpStatusCode
        /// </summary>
        private static int GetHttpStatusCode(ResultStatusEnum status)
        {
            return status switch
            {
                // Success responses
                ResultStatusEnum.Success => 200,
                ResultStatusEnum.Created => 201,
                ResultStatusEnum.Accepted => 202,
                
                // Client error responses
                ResultStatusEnum.BadRequest => 400,
                ResultStatusEnum.ValidationFailed => 400,
                ResultStatusEnum.NotFound => 404,
                ResultStatusEnum.Unauthorized => 401,
                ResultStatusEnum.Forbidden => 403,
                ResultStatusEnum.Conflict => 409,
                
                // Server error responses
                ResultStatusEnum.InternalError => 500,
                ResultStatusEnum.ServiceUnavailable => 503,
                ResultStatusEnum.DatabaseError => 500,
                ResultStatusEnum.ThirdPartyError => 502,
                
                _ => 500
            };
        }
    }
}