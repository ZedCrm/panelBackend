// App/Object/Shop/ProductApp/ProductBusinessService.cs
using App.Contracts.Object.Shop.ProductCon;
using App.utility;
using AutoMapper;
using ConfApp;
using Domain.Objects.Shop;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using System.Linq.Expressions;

namespace App.Object.Shop.ProductApp
{
    public class ProductBusinessService : BaseService<Product>
    {
        private readonly IMapper _mapper;

        public ProductBusinessService(MyContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<StatusResult> ValidateUniqueProductCodeAsync(string productCode, int? excludeId = null)
        {
            var query = GetActiveQuery();
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            var exists = await query.AnyAsync(p => p.ProductCode == productCode);
            if (exists)
                return ResultFactory.Status(ResultStatusEnum.Conflict, MessageApp.DuplicateField("کد محصول"));

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        public async Task<ListDataResult<ProductView>> SearchAsync(ProductSearchCriteria criteria)
        {
            var query = GetActiveQuery()
                .Include(p => p.CountType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Name))
                query = query.Where(p => p.Name.Contains(criteria.Name));

            if (criteria.MinPrice.HasValue && criteria.MinPrice.Value > 0)
                query = query.Where(p => p.Price >= criteria.MinPrice.Value);

            if (criteria.MaxPrice.HasValue && criteria.MaxPrice.Value > 0)
                query = query.Where(p => p.Price <= criteria.MaxPrice.Value);

            var total = await query.CountAsync();

            var products = await query
                .Skip(criteria.CalculateSkip())
                .Take(criteria.PageSize)
                .ToListAsync();

            var views = _mapper.Map<List<ProductView>>(products);

            return ResultFactory.List(ResultStatusEnum.Success, views, total, criteria);
        }
    }
}