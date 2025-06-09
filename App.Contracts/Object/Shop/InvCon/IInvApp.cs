using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Shop.InvCon
{
    public interface IInvApp
    {
         public Task<OPTResult<invView>> GetAll(Pagination pagination);
        public Task<OPTResult<invUpdate>> GetById(int id);
        public Task<OPTResult<invCreate>> Create(invCreate createData);
        public Task<OPTResult<invUpdate>> Update(invUpdate updateData);
        public Task<OPTResult<invView>> DeleteBy(List<int> ids);
    }
}