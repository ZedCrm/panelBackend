using App.Contracts.Object.Shop.ProductCon;
using App.Object.Base;
using App.utility;
using AutoMapper;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace App.Object.Shop.ProductApp
{
    public class ProductApp : CrudService<Product, ProductView, ProductCreate, ProductUpdate, int>,
                              IProductApp
    {
        private readonly IProductRep _productRep;
        private readonly IMapper _mapper;

        public ProductApp(IProductRep productRep, IMapper mapper)
            : base(productRep, mapper)
        {
            _productRep = productRep;
            _mapper = mapper;
        }

        /*=== CRUD یک‌خطی ===*/
        public Task<ApiResult<List<ProductView>>> GetAll(Pagination pagination) => base.GetAllAsync(pagination);
        public Task<ApiResult<ProductUpdate>> GetById(int id) => base.GetByIdAsync(id);
        public Task<ApiResult> Create(ProductCreate dto) => base.CreateAsync(dto);
        public Task<ApiResult> DeleteBy(List<int> ids) => base.DeleteAsync(ids);
        public Task<ApiResult> Update(ProductUpdate dto) => base.UpdateAsync(dto);

        /*=== متد اختصاصی ===*/
        public async Task<ApiResult<List<ProductView>>> SearchProducts(ProductSearchCriteria criteria)
        {
            Expression<Func<Product, bool>> filter = p => true;

            if (!string.IsNullOrWhiteSpace(criteria.Name))
                filter = filter.And(p => p.Name.Contains(criteria.Name));

            if (criteria.MinPrice.HasValue && criteria.MinPrice.Value != 0)
                filter = filter.And(p => p.Price >= criteria.MinPrice.Value);

            if (criteria.MaxPrice.HasValue && criteria.MaxPrice.Value != 0)
                filter = filter.And(p => p.Price <= criteria.MaxPrice.Value);

            var data = await _productRep.GetFilteredAsync(filter, criteria);
            var views = _mapper.Map<List<ProductView>>(data);
            var total = await _productRep.CountAsync(filter);

            return ApiResult<List<ProductView>>.PagedSuccess(views, total,
                                                             criteria.PageNumber,
                                                             criteria.PageSize);
        }
    }

    public interface IProductRep : IBaseRep<Product, int> { }
}