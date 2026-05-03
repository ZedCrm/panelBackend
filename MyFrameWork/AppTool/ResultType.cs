namespace MyFrameWork.AppTool.ResultType
{
public  class StatusResult{

    public int Status { get; set; }
    
    public List<string>? Messeges { get; set; }

    public StatusResult(int statusid,List<string>? messeges)
    {
        this.Status = statusid ; this.Messeges = messeges;
        
    }

}


    public  class  SingleDataResult<T> : StatusResult where T : class
    {

        public T?   SingleData{ get; set; }
        public SingleDataResult(int statusid, List<string>? messeges , T? singleData) : base(statusid, messeges)
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

        public ListDataResult(int statusid, List<string>? messeges, List<T>? data, int? totalRecords,  int? pageNumber, int? pageSize) : base(statusid, messeges)
        {
              Data = data;
        TotalRecords = totalRecords;
        PageNumber = pageNumber;
        PageSize = pageSize;
        }
    }

    public   class FileResult : StatusResult
{
         public string? FilePath  { get; set; }
        public  FileResult(int statusid, List<string>? messeges , string filePath) : base(statusid, messeges)
        {
            FilePath = filePath ;
        }

       

   
}

public static class ResultFactory
{
    // ایجاد StatusResult ساده
    public static StatusResult Status(int statusId, params string[] messages)
        => new StatusResult(statusId, messages.ToList());

    // ایجاد SingleDataResult
    public static SingleDataResult<T> Single<T>(int statusId, T data, params string[] messages)
        where T : class
        => new SingleDataResult<T>(statusId, messages.ToList(), data);

    // ایجاد ListDataResult
    public static ListDataResult<T> List<T>(
        int statusId,
        List<T> data,
        int totalRecords,
        int pageNumber,
        int pageSize,
        params string[] messages
    ) where T : class
        => new ListDataResult<T>(statusId, messages.ToList(), data, totalRecords, pageNumber, pageSize);

    // ایجاد FileResult
    public static FileResult File(int statusId, string filePath, params string[] messages)
        => new FileResult(statusId, messages.ToList(), filePath);
}



}