namespace API.utility
{
    public class RequirePermissionForEntityAttribute : Attribute
    {
        public Type EntityType { get; }
        public string Action { get; }

        public RequirePermissionForEntityAttribute(Type entityType, string action)
        {
            EntityType = entityType;
            Action = action;
        }
    }
}
