using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Contracts.Object.Shop.InvCon
{
    public interface IInvApp : ICrudService<InvView, InvCreate, InvUpdate, int>
    {


    }
}