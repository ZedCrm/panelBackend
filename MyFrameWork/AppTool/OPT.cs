using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFrameWork.AppTool
{
    public class OPTOld
    {
        public bool IsSucceeded { get; set; }
        public  string Message { get; set; }
        public OPTOld()
        {
            IsSucceeded = true;
        }

        public OPTOld Succeeded(string message = "عملیات با موفقیت انجام شد")
        {
            IsSucceeded = true;
            Message = message;
            return this;
        }
        public OPTOld Failed(string message)
        {
            IsSucceeded = false;
            Message = message;
            return this;
        }
    }
}
