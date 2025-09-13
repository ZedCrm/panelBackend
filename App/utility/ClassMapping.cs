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


            CreateMap<User, UsersView>();
            CreateMap<UsersCreat,User>().ForMember(p => p.Id, o => o.Ignore());
            CreateMap<User, UsersCreat>();


        }
    }
}
