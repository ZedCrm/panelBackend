// MyFrameWork/AppTool/ApiResultExtensions.cs
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;


namespace API.utility
{
    public static class ApiResultExtensions
    {
        public static IActionResult ToActionResult(this ApiResult result)
        {
            if (result == null) return new StatusCodeResult(500);

            var content = new
            {
                success = result.IsSucceeded,
                message = result.Message,
                errors = result.Errors,
                data = (object?)null,
                pagination = result.TotalRecords != null ? new
                {
                    result.TotalRecords,
                    result.TotalPages,
                    result.PageNumber,
                    result.PageSize
                } : null
            };

            return result.IsSucceeded
                ? new OkObjectResult(content) { StatusCode = result.StatusCode }
                : new ObjectResult(content) { StatusCode = result.StatusCode };
        }

        public static IActionResult ToActionResult<T>(this ApiResult<T> result)
        {
            if (result == null) return new StatusCodeResult(500);

            var content = new
            {
                success = result.IsSucceeded,
                message = result.Message,
                errors = result.Errors,
                data = result.Data,
                pagination = result.TotalRecords != null ? new
                {
                    result.TotalRecords,
                    result.TotalPages,
                    result.PageNumber,
                    result.PageSize
                } : null
            };

            return result.IsSucceeded
                ? new OkObjectResult(content) { StatusCode = result.StatusCode }
                : new ObjectResult(content) { StatusCode = result.StatusCode };
        }
    }
}