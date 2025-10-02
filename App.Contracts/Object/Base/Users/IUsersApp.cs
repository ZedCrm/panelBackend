using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Base.Users
{
    public interface IUsersApp
    {
        
       // public Task<OPTResult<UsersView>> SearchProducts(ProductSearchCriteria criteria);
        Task<OPTResult<UsersView>> GetAll(Pagination pagination, int userId);
        Task<OPT> Create(UsersCreat objectCreate);
        Task<OPT> DeleteBy(List<int> objectids);
        Task<OPTResult<UsersCreat>> GetById(int id);
        Task<OPT> Update(UsersUpdate objectView);
        
    }
}