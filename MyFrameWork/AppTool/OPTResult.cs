using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFrameWork.AppTool
{
    public class OPTResult<T> where T : class
    {
        public bool? IsSucceeded { get; set; }
        public string? Message { get; set; }
        public List<T>? Data { get; set; }  // لیست داده‌ها  
        public int? TotalRecords { get; set; } // تعداد کل رکوردها  
        public int? TotalPages { get; set; }   // تعداد کل صفحات  
        public int? PageNumber { get; set; }    // شماره صفحه جاری  
        public int? PageSize { get; set; }      // اندازه صفحه  

        public OPTResult()
        {
            IsSucceeded = true;
             // مقداردهی اولیه به لیست داده‌ها  
        }

        public OPTResult<T> Succeeded(string message = "عملیات با موفقیت انجام شد")
        {
            IsSucceeded = true;
            Message = message;
            return this;
        }

        public OPTResult<T> Failed(string message)
        {
            IsSucceeded = false;
            Message = message;
            return this;
        }
    }
}
