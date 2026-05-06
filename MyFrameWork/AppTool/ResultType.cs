namespace MyFrameWork.AppTool.ResultType
{
    public enum ResultStatusEnum
{
    // Success
    Success = 1,
    Created = 2,
    Accepted = 3,
    
    // Client Errors (بیزینس)
    BadRequest = 100,
    ValidationFailed = 101,
    NotFound = 102,
    Unauthorized = 103,
    Forbidden = 104,
    Conflict = 105,
    
    // Server Errors (فنی)
    InternalError = 500,
    ServiceUnavailable = 501,
    DatabaseError = 502,
    ThirdPartyError = 503
}



public  class StatusResult{

    public ResultStatusEnum Status { get; set; }  
    
    public IEnumerable<string>? Messages { get; set; }
     public bool IsSuccess => 
        Status == ResultStatusEnum.Success || 
        Status == ResultStatusEnum.Created || 
        Status == ResultStatusEnum.Accepted;
    

    public StatusResult(ResultStatusEnum statusid,IEnumerable<string>? messages)
    {
        this.Status = statusid ; this.Messages = messages;
        
    }
    

}


    public  class  SingleDataResult<T> : StatusResult 
    {

        public T?   SingleData{ get; set; }
        public SingleDataResult(ResultStatusEnum statusid, IEnumerable<string>? messages , T? singleData) : base(statusid, messages)
        {
                this.SingleData = singleData;

        }

        
    }


    public  class ListDataResult<T> : StatusResult where T : class
    {
        

        public List<T>? Data { get; set; }
        public int? TotalRecords { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public ListDataResult(ResultStatusEnum statusid,IEnumerable<string>? messages , List<T>? data, int? totalRecords, Pagination pagination ) : base(statusid, messages)
        {
              Data = data;
        TotalRecords = totalRecords;
        PageNumber = pagination.PageNumber;
        PageSize = pagination.PageSize;
        }
    }

    public   class FileResult : StatusResult
{
         public string? FilePath  { get; set; }
        public  FileResult(ResultStatusEnum statusid, IEnumerable<string>? messeges , string filePath) : base(statusid, messeges)
        {
            FilePath = filePath ;
        }

       

   
}

public static class ResultFactory
{
    // ایجاد StatusResult ساده
    public static StatusResult Status(ResultStatusEnum statusId, params string[] Messages)
        => new StatusResult(statusId, Messages.ToList());

    // ایجاد SingleDataResult
    public static SingleDataResult<T> Single<T>(ResultStatusEnum statusId, T data, params string[] Messages)
        
        => new SingleDataResult<T>(statusId, Messages.ToList(), data);

    // ایجاد ListDataResult
    public static ListDataResult<T> List<T>(
        ResultStatusEnum statusId,
        List<T> data,
        int totalRecords,
        Pagination pagination ,
        params string[] message
    ) where T : class
        => new ListDataResult<T>(statusId, message, data, totalRecords, pagination);




            public static ListDataResult<T> List<T>(
        ResultStatusEnum statusId,
        List<T> data,
        params string[] message
    ) where T : class
        => new ListDataResult<T>(statusId, message, data,0,new Pagination());
    // ایجاد FileResult
    public static FileResult File(ResultStatusEnum statusId, string filePath, params string[] Messages)
        => new FileResult(statusId, Messages.ToList(), filePath);
}




    public static class ResultHelper
    {
        // ========== موفقیت ==========
        public static StatusResult Success(string message = "عملیات با موفقیت انجام شد")
            => ResultFactory.Status(ResultStatusEnum.Success, message);

        public static StatusResult Created(string message = "با موفقیت ایجاد شد")
            => ResultFactory.Status(ResultStatusEnum.Created, message);

        public static StatusResult Accepted(string message = "درخواست پذیرفته شد")
            => ResultFactory.Status(ResultStatusEnum.Accepted, message);

        // ========== خطاهای کلاینت (4xx) ==========
        public static StatusResult BadRequest(string message)
            => ResultFactory.Status(ResultStatusEnum.BadRequest, message);

        public static StatusResult NotFound(string entityName, object? id = null)
        {
            var msg = id == null 
                ? $"{entityName} یافت نشد" 
                : $"{entityName} با شناسه {id} یافت نشد";
            return ResultFactory.Status(ResultStatusEnum.NotFound, msg);
        }

        public static StatusResult Unauthorized(string message = "لطفاً وارد سیستم شوید")
            => ResultFactory.Status(ResultStatusEnum.Unauthorized, message);

        public static StatusResult Forbidden(string permission)
            => ResultFactory.Status(ResultStatusEnum.Forbidden, $"شما دسترسی '{permission}' را ندارید");

        public static StatusResult Conflict(string fieldName, string value)
            => ResultFactory.Status(ResultStatusEnum.Conflict, $"{fieldName} '{value}' قبلاً ثبت شده است");

        public static StatusResult ValidationFailed(params string[] errors)
            => ResultFactory.Status(ResultStatusEnum.ValidationFailed, errors);

        // ========== خطاهای سرور (5xx) ==========
        public static StatusResult InternalError(string message = "خطای داخلی سرور رخ داده است")
            => ResultFactory.Status(ResultStatusEnum.InternalError, message);

        public static StatusResult DatabaseError(string message = "خطا در ارتباط با پایگاه داده")
            => ResultFactory.Status(ResultStatusEnum.DatabaseError, message);
    }

}