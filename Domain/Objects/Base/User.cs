using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Objects.Chat;

namespace Domain.Objects.Base
{
    public class User : BaseDomain
    {

        public string FullName { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string? ProfilePictureUrl { get; set; }  // جدید: مسیر عکس پروفایل
        public UserStatus Status { get; set; } = UserStatus.Offline;  // جدید: وضعیت
        public DateTime? LastSeen { get; set; }  // جدید: آخرین بازدید
        public List<UserRole> UserRoles { get; set; }

        public List<Message> SentMessages { get; set; } = new();
        public List<Message> ReceivedMessages { get; set; } = new();
        public List<ChatGroup> CreatedGroups { get; set; } = new();
        public List<ChatGroupMember> GroupMemberships { get; set; } = new();
        public List<GroupMessage> GroupMessages { get; set; } = new();

    }

    public enum UserStatus  // جدید: enum برای وضعیت
    {
        Offline = 1,  // لاگ اوت کامل
        Inactive = 2,  // لاگین ولی صفحه اکتیو نیست
        Online = 3    // آنلاین و در صفحه
    }

    public class Role : BaseDomain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
    }

    public class Permission : BaseDomain
    {
        public int Id { get; set; }
        public int Category { get; set; }
        public string Name { get; set; } // مثل ViewProduct, EditProduct
        public List<RolePermission> RolePermissions { get; set; }
    }

    public class UserRole : BaseDomain
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class RolePermission : BaseDomain
    {
        public int Id { get; set; }
        public Role Role { get; set; }
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}


