using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Objects.Chat;

namespace Domain.Objects.Base
{
    [Table("Users")]
    public class User : BaseDomain
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = default!;

        [Required]
        [EmailAddress]
        [StringLength (100)]
        public string Email { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = default!;
        [StringLength(200)]
        public string? ProfilePictureUrl { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Offline;

        public DateTime? LastSeen { get; set; }

        // Navigation properties (بدون Attribute)
        public List<UserRole> UserRoles { get; set; }
        public List<Message> SentMessages { get; set; } = new();
        public List<Message> ReceivedMessages { get; set; } = new();
        public List<ChatGroup> CreatedGroups { get; set; } = new();
        public List<ChatGroupMember> GroupMemberships { get; set; } = new();
        public List<GroupMessage> GroupMessages { get; set; } = new();
    }

    public enum UserStatus
    {
        Offline = 1,
        Inactive = 2,
        Online = 3
    }

    [Table("Roles")]
    public class Role : BaseDomain
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<UserRole> UserRoles { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
    }

    [Table("Permissions")]
    public class Permission : BaseDomain
    {
        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<RolePermission> RolePermissions { get; set; }
    }

    [Table("UserRoles")]
    public class UserRole : BaseDomain
    {
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
    }

    [Table("RolePermissions")]
    public class RolePermission : BaseDomain
    {
        [Required]
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }

        [Required]
        public int PermissionId { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public Permission Permission { get; set; }
    }
} 