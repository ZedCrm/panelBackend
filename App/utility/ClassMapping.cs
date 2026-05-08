using App.Contracts.Object.Base;
using App.Contracts.Object.Base.Roles;
using App.Contracts.Object.Base.Users;
using App.Contracts.Object.Chat;
using App.Contracts.Object.Shop.CountTypeCon;
using App.Contracts.Object.Shop.InvCon;
using App.Contracts.Object.Shop.ProductCon;
using AutoMapper;
using Domain.Objects.Base;
using Domain.Objects.Chat;
using Domain.Objects.Shop;

public class ClassMapping : Profile
{
    public ClassMapping()
    {
        // ========== User Mappings ==========
        CreateMap<User, UsersView>()
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => src.LastSeen));

        CreateMap<UsersCreat, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<UsersUpdate, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

        // ========== Role Mappings ==========
        CreateMap<Role, RoleView>()
            .ForMember(dest => dest.Rolename, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission)));

        CreateMap<RoleCreate, Role>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

        CreateMap<RoleUpdate, Role>()
            .ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

        CreateMap<Permission, PermissionView>()
            .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Name));

        // ========== Chat Mappings ==========
        CreateMap<Message, MessageView>()
            .ForMember(dest => dest.SenderName, opt => opt.Ignore())  // بعداً مقداردهی می‌شود
            .ForMember(dest => dest.IsMine, opt => opt.Ignore());

        // ========== Product Mappings ==========
        CreateMap<Product, ProductView>();
        CreateMap<ProductCreate, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<ProductUpdate, Product>()
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore());

        // ========== CountType Mappings ==========
        CreateMap<CountType, CountTypeView>();
        CreateMap<CountTypeCreate, CountType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.products, opt => opt.Ignore());

        // ========== Inv Mappings ==========
        CreateMap<Inv, InvView>();
        CreateMap<InvCreate, Inv>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<InvUpdate, Inv>();

        // ========== Person Mappings ==========
        CreateMap<Person, PersonView>();
        CreateMap<PersonCreate, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore());
        CreateMap<PersonUpdate, Person>();
    }
}