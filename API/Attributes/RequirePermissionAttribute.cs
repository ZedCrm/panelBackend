using Microsoft.AspNetCore.Mvc;

namespace API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePermissionAttribute : Attribute
    {
        public string Permission { get; }
        public RequirePermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
}