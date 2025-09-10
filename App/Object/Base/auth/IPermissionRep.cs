using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Object.Base.auth
{
    public interface IPermissionRep 
    {
        Task<bool> HasPermissionAsync(int userId, string permission);
    }
}