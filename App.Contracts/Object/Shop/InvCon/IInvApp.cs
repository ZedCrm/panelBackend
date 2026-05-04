using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Contracts.Object.Shop.InvCon
{
    public interface IInvApp
    {


        public Task<ListDataResult<InvView>> GetAll(Pagination pagination);
        public Task<SingleDataResult<InvUpdate>> GetById(int id);
        public Task<StatusResult> Create(InvCreate dto);
        public Task<StatusResult> DeleteBy(List<int> ids);
        public Task<StatusResult> Update(InvUpdate dto);


    }
}