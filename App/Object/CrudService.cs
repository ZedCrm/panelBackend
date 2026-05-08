// App/Object/CrudService.cs
using App.utility;
using AutoMapper;
using ConfApp;
using Domain.Objects;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using System.Linq.Expressions;

namespace App.Object
{
    public abstract class CrudService<TEntity, TDto, TCreate, TUpdate, TKey> : BaseService<TEntity>
        where TEntity : BaseDomain
        where TDto : class
        where TCreate : class
        where TUpdate : class
    {
        protected readonly IMapper _mapper;

        protected CrudService(MyContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public virtual async Task<ListDataResult<TDto>> GetAllAsync(Pagination pagination)
        {
            var query = GetActiveQuery().AsNoTracking();

            if (!string.IsNullOrEmpty(pagination.SortBy))
            {
                query = ApplySorting(query, pagination.SortBy, pagination.SortDirection);
            }

            var total = await query.CountAsync();
            var entities = await query
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            var data = _mapper.Map<List<TDto>>(entities);
            return ResultFactory.List(ResultStatusEnum.Success, data, total, pagination);
        }

        public virtual async Task<SingleDataResult<TUpdate>> GetByIdAsync(TKey id)
        {
            var entity = await GetEntityOrNullAsync(e => e.Id.Equals(id));
            if (entity == null)
                return ResultFactory.Single<TUpdate>(ResultStatusEnum.NotFound, null, MessageApp.NotFoundItem(typeof(TEntity).Name));

            var dto = _mapper.Map<TUpdate>(entity);
            return ResultFactory.Single(ResultStatusEnum.Success, dto);
        }

        public virtual async Task<StatusResult> CreateAsync(TCreate dto)
        {
            var validation = ValidateModel(dto);
            if (!validation.IsSuccess) return validation;

            var entity = _mapper.Map<TEntity>(dto);
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return ResultFactory.Status(ResultStatusEnum.Success, MessageApp.CustomAddsuccses("ایجاد"));
        }

        public virtual async Task<StatusResult> UpdateAsync(TUpdate dto)
        {
            var validation = ValidateModel(dto);
            if (!validation.IsSuccess) return validation;

            var idProperty = typeof(TUpdate).GetProperty("Id");
            if (idProperty == null)
                return ResultFactory.Status(ResultStatusEnum.ValidationFailed, "DTO فاقد خاصیت Id است");

            var id = (TKey)idProperty.GetValue(dto)!;
            var entity = await GetEntityOrNullAsync(e => e.Id.Equals(id));

            if (entity == null)
                return ResultFactory.Status(ResultStatusEnum.NotFound, MessageApp.NotFoundItem(typeof(TEntity).Name));

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            return ResultFactory.Status(ResultStatusEnum.Success, MessageApp.CustomSuccess("ویرایش"));
        }

        public virtual async Task<StatusResult> DeleteAsync(List<TKey> ids)
        {
            if (ids == null || ids.Count == 0)
                return ResultFactory.Status(ResultStatusEnum.BadRequest, "هیچ شناسه‌ای ارسال نشده است.");

            var intIds = ids.Select(id => Convert.ToInt32(id)).ToList();
            var entities = await _dbSet.Where(e => intIds.Contains(e.Id)).ToListAsync();

            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();

            return ResultFactory.Status(ResultStatusEnum.Success, $"{entities.Count} {MessageApp.CustomSuccess("حذف")}");
        }

        private static IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, string sortBy, bool ascending)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), property.Type);

            return (IQueryable<TEntity>)method.Invoke(null, new object[] { query, lambda })!;
        }
    }
}