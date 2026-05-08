using App.Contracts.Object.Base.Roles;
using App.Contracts.Object.Base.Users;
using AutoMapper;
using Domain.Objects.Base;

public class ClassMapping : Profile
{
    public ClassMapping()
    {
        // مپ‌های اختصاصی (برای خواصی که نام متفاوت دارند) را دستی بنویسید
        CreateMap<User, UsersView>()
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl));

        CreateMap<Role, RoleView>()
    .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission)));
        CreateMap<RoleCreate, Role>();
        CreateMap<RoleUpdate, Role>()
            .ForMember(dest => dest.RolePermissions, opt => opt.Ignore()); // دستی مدیریت می‌شود
        CreateMap<Permission, PermissionView>();



        // اما بقیه مپ‌ها را خودکار انجام بده
        AutoMapAllEntitiesAndDtos();
    }

    private void AutoMapAllEntitiesAndDtos()
    {
        var domainTypes = typeof(Domain.Objects.BaseDomain).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Domain.Objects.BaseDomain)));

        var dtoTypes = typeof(App.Contracts.Object.Base.Users.UsersView).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && (t.Name.EndsWith("View") || t.Name.EndsWith("Dto") || t.Name.EndsWith("Create") || t.Name.EndsWith("Update")));

        foreach (var entity in domainTypes)
        {
            // DTO همنام با View یا Create یا Update
            var possibleDtos = dtoTypes.Where(d =>
                d.Name == entity.Name + "View" ||
                d.Name == entity.Name + "Create" ||
                d.Name == entity.Name + "Update");

            foreach (var dto in possibleDtos)
            {
                if (dto.Name.EndsWith("Create") || dto.Name.EndsWith("Update"))
                    CreateMap(dto, entity).ForMember("Id", opt => opt.Ignore());
                else
                    CreateMap(entity, dto);
            }
        }
    }
}