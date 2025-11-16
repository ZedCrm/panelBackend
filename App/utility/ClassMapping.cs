// فایل: App/ClassMapping.cs
using App.Contracts.Object.Base.Users;
using App.Contracts.Object.Shop.CountTypeCon;
using App.Contracts.Object.Shop.InvCon;
using App.Contracts.Object.Shop.ProductCon;
using AutoMapper;
using Domain.Objects.Base;
using Domain.Objects.Shop;

namespace App.utility
{
    // این کلاس برای نگاشت (Map) بین مدل‌های دامنه (Entity) و مدل‌های انتقال داده (DTO) استفاده می‌شود
    public class ClassMapping : Profile
    {
        public ClassMapping()
        {
            // نگاشت از مدل دامنه Product به مدل نمایشی ProductView
            CreateMap<Product, ProductView>();

            // نگاشت از مدل ساخت ProductCreate به مدل دامنه Product
            CreateMap<ProductCreate, Product>()
                // ویژگی Id را نادیده می‌گیریم چون در زمان ایجاد توسط دیتابیس مقداردهی می‌شود
                .ForMember(p => p.Id, o => o.Ignore());
            CreateMap<Product, ProductUpdate>();

            // نگاشت از مدل دامنه CountType به مدل نمایشی CountTypeViw
            CreateMap<CountType, CountTypeView>();


            // نگاشت از مدل دامنه CountType به مدل ساخت CountTypeView
            CreateMap<CountTypeView, CountType>();


            // نگاشت از مدل ساخت CountTypeCreate به مدل دامنه CountType
            CreateMap<CountTypeCreate, CountType>()
                // ویژگی Id باید در ساخت جدید مقدار نداشته باشد چون توسط دیتابیس مقداردهی می‌شود
                .ForMember(p => p.Id, o => o.Ignore());


        // مپینگ برای UsersView (برای GetAll) - بدون RoleIds
            CreateMap<User, UsersView>()
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl)); 

            // مپینگ برای UsersUpdate (برای GetById) - شامل RoleIds
            CreateMap<User, UsersUpdate>()
                .ForMember(dest => dest.RoleIds, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.RoleId).ToList()))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl))
                .ForMember(dest => dest.Password, opt => opt.Ignore()); 

            // مپینگ برای UsersCreat به User
            CreateMap<UsersCreat, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Password) ? null : BCrypt.Net.BCrypt.HashPassword(src.Password)))
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.RoleIds.Select(roleId => new UserRole { RoleId = roleId }).ToList()))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl));

            // مپینگ برای UsersUpdate به User
            CreateMap<UsersUpdate, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Password) ? null : BCrypt.Net.BCrypt.HashPassword(src.Password)))
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.RoleIds.Select(roleId => new UserRole { RoleId = roleId }).ToList()))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl));

            // مپینگ معکوس (اختیاری، در صورت نیاز)
            CreateMap<User, UsersCreat>()
                .ForMember(dest => dest.RoleIds, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.RoleId).ToList()))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl));


            CreateMap<User,UserList>();


            CreateMap<Inv , InvView>();


        }
    }
}
