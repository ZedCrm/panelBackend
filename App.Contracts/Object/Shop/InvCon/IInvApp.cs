using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Shop.InvCon
{
    public interface IInvApp
    {


        public Task<ApiResult<List<InvView>>> GetAll(Pagination pagination);
        public Task<ApiResult<InvUpdate>> GetById(int id);
        public Task<ApiResult> Create(InvCreate dto);
        public Task<ApiResult> DeleteBy(List<int> ids);
        public Task<ApiResult> Update(InvUpdate dto);


    }
}