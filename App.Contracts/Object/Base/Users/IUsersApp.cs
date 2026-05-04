using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Contracts.Object.Base.Users
{
public interface IUsersApp
    {
Task<ListDataResult<UsersView>>             GetAll(Pagination pagination);
        Task<SingleDataResult<UsersUpdate>>         GetById(int id);
        Task<StatusResult>                      CreateAsync(UsersCreat dto);
        Task<StatusResult>                      UpdateAsync(UsersUpdate dto);
        Task<StatusResult>                      DeleteBy(List<int> ids);
        Task<SingleDataResult<UserCreateFormData>>  CreateForm();
        Task<StatusResult>                      KeepAlive(int userId);
        Task<ListDataResult<UserList>>      GetList();
    }
}