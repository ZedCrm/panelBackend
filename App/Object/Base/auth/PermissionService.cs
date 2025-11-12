using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Contracts.Object.Base.auth;

namespace App.Object.Base.auth
{
    public class PermissionService :IPermissionService
    {
       private readonly IPermissionRep _permissionRep;

        public PermissionService (IPermissionRep permissionRep)
        {
            _permissionRep = permissionRep;
        }

        public async Task<bool> HasPermissionAsync(int userId, string permission)
        {
            return await _permissionRep.HasPermissionAsync(userId, permission);
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId){
            return await _permissionRep.GetUserPermissionsAsync(userId);
        }

    }
}