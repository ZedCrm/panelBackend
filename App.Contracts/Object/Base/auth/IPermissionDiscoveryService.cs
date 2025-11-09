using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Contracts.Object.Base.auth
{
    public interface IPermissionDiscoveryService
    {
        IEnumerable<string> GetAllPermissionNames();
    }
}