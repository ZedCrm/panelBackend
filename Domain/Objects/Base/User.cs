using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Objects.Base
{
    public class User : BaseDomain
    {

        public string FullName { get; set; } = default!;
        public string Username { get; set; }= default!   ;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public List<UserRole> UserRoles { get; set; }

    }


    public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public List<UserRole> UserRoles { get; set; }
    public List<RolePermission> RolePermissions { get; set; }
}

public class Permission
{
    public int Id { get; set; }
    public int Category {get; set;}
    public string Name { get; set; } // مثل ViewProduct, EditProduct
    public List<RolePermission> RolePermissions { get; set; }
}

public class UserRole
{
    public int Id { get; set; }
    public User User { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
}

public class RolePermission
{
    public int Id { get; set; }
    public Role Role { get; set; }
    public int PermissionId { get; set; }
    public Permission Permission { get; set; }
}
}


