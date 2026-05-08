using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Contracts.Object.Base.Users
{
public interface IUsersApp : ICrudService<UsersView,UsersCreat,UsersUpdate , int>
    {
        
        Task<SingleDataResult<UserCreateFormData>>  CreateForm();
        Task<StatusResult>                      KeepAlive(int userId);
        Task<ListDataResult<UserList>>      GetList();
    }
}