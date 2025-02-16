using App.Contracts.Object.Shop.ProductCon;
using AutoMapper;
using Domain.Objects.Shop;

namespace App
{
    public class ClassMapping : Profile
    {

        public ClassMapping()
        {
            CreateMap<Product, ProductView>();
            CreateMap<ProductCreate , Product>().
                ForMember(p=>p.Id , o=>o.MapFrom(pc=>pc.CountTypeId)).ForMember(p => p.Id, o => o.Ignore());
        }
    }
}
