using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Contracts.Object.Base.Roles
{
    // RoleView.cs
    public class RoleView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PermissionView> Permissions { get; set; } = new();
    }

    public class PermissionView
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string Category { get; set; }
    }

    public class RoleCreate
    {
        public string Name { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class RoleUpdate : RoleCreate
    {
        public int Id { get; set; }
    }






}
