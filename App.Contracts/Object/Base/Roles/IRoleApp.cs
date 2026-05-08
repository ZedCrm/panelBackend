using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Contracts.Object.Base.Roles
{
    public interface IRoleApp : ICrudService<RoleView, RoleCreate, RoleUpdate, int>
    {
        Task<List<PermissionView>> GetAllPermissionsAsync();
    }

}
