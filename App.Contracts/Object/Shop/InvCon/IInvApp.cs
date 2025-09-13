using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Shop.InvCon
{
    public interface IInvApp
    {
        public Task<OPTResult<InvView>> GetAll(Pagination pagination);
        public Task<OPTResult<InvUpdate>> GetById(int id);
        public Task<OPTResult<InvCreate>> Create(InvCreate createData);
        public Task<OPTResult<InvUpdate>> Update(InvUpdate updateData);
        public Task<OPTResult<InvView>> DeleteBy(List<int> ids);
    }
}