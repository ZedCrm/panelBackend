using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Base.Users
{
public interface IUsersApp
    {
        Task<ApiResult> GetAll(Pagination pagination);
        Task<ApiResult> Create(UsersCreat objectCreate);
        Task<ApiResult> DeleteBy(List<int> objectids);
        Task<ApiResult> GetById(int id);
        Task<ApiResult> Update(UsersUpdate objectView);
        Task<ApiResult> CreateForm();
        Task<ApiResult> KeepAlive(int userId);
    }
}