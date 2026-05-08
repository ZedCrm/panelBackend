// App/Object/Shop/ProductApp/ProductApp.cs
using App.Contracts.Object.Shop.ProductCon;
using App.utility;
using AutoMapper;
using Domain.Objects.Shop;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using ConfApp;

namespace App.Object.Shop.ProductApp
{
    public class ProductApp : CrudService<Product, ProductView, ProductCreate, ProductUpdate, int>, IProductApp
    {
        private readonly ProductBusinessService _productBusiness;

        public ProductApp(MyContext context, IMapper mapper, ProductBusinessService productBusiness)
            : base(context, mapper)
        {
            _productBusiness = productBusiness;
        }

        public override async Task<StatusResult> CreateAsync(ProductCreate dto)
        {
            var uniqueCheck = await _productBusiness.ValidateUniqueProductCodeAsync(dto.ProductCode);
            if (!uniqueCheck.IsSuccess) return uniqueCheck;

            return await base.CreateAsync(dto);
        }

        public override async Task<StatusResult> UpdateAsync(ProductUpdate dto)
        {
            var uniqueCheck = await _productBusiness.ValidateUniqueProductCodeAsync(dto.ProductCode, dto.Id);
            if (!uniqueCheck.IsSuccess) return uniqueCheck;

            return await base.UpdateAsync(dto);
        }

        public Task<ListDataResult<ProductView>> SearchProducts(ProductSearchCriteria criteria)
            => _productBusiness.SearchAsync(criteria);
    }
}