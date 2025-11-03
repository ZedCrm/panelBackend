public class ApiResult<T> where T : class
{
    public bool IsSucceeded { get; set; } = true;
    public int StatusCode { get; set; } = 200;
    public List<string> Messages { get; set; } = new();  // پیام‌های موفقیت
    public List<string> Errors { get; set; } = new();    // خطاها
    public T? Data { get; set; }
    public int? TotalRecords { get; set; }
    public int? TotalPages { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }

    // موفقیت با داده + پیام
    public static ApiResult<T> Success(T data, int statusCode = 200, string? message = null)
    {
        return new ApiResult<T>
        {
            IsSucceeded = true,
            StatusCode = statusCode,
            Data = data,
            Messages = message != null ? new List<string> { message } : new()
        };
    }

    // موفقیت بدون داده
    public static ApiResult<T> Success(int statusCode = 200, string? message = null)
    {
        return new ApiResult<T>
        {
            IsSucceeded = true,
            StatusCode = statusCode,
            Messages = message != null ? new List<string> { message } : new()
        };
    }

    // شکست
    public static ApiResult<T> Failed(int statusCode, params string[] errors)
    {
        return new ApiResult<T>
        {
            IsSucceeded = false,
            StatusCode = statusCode,
            Errors = errors.ToList()
        };
    }

    public static ApiResult<T> Failed(int statusCode, List<string> errors)
    {
        return new ApiResult<T>
        {
            IsSucceeded = false,
            StatusCode = statusCode,
            Errors = errors
        };
    }

    // صفحه‌بندی + پیام
    public static ApiResult<T> SuccessPaged(
        List<T> data,
        int totalRecords,
        int pageNumber,
        int pageSize,
        int statusCode = 200,
        string? message = null)
    {
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        return new ApiResult<T>
        {
            IsSucceeded = true,
            StatusCode = statusCode,
            Data = data as T,
            Messages = message != null ? new List<string> { message } : new(),
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}