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
        Task<ApiResult<UsersView>> GetAll(Pagination pagination);
        Task<ApiResult<object>> Create(UsersCreat objectCreate);
        Task<ApiResult<object>> DeleteBy(List<int> objectids);
        Task<ApiResult<UsersUpdate>> GetById(int id);
        Task<ApiResult<object>> Update(UsersUpdate objectView);
        Task<ApiResult<UserCreateFormData>> CreateForm();
        Task<ApiResult<object>> KeepAlive(int userId);
    }
}