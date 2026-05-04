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



}