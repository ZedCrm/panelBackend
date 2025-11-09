using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Base.Users
{
public interface IUsersApp
    {
Task<ApiResult<List<UsersView>>>             GetAll(Pagination pagination);
        Task<ApiResult<UsersUpdate>>         GetById(int id);
        Task<ApiResult>                      Create(UsersCreat dto);
        Task<ApiResult>                      Update(UsersUpdate dto);
        Task<ApiResult>                      DeleteBy(List<int> ids);
        Task<ApiResult<UserCreateFormData>>  CreateForm();
        Task<ApiResult>                      KeepAlive(int userId);
    }
}