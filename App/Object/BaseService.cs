// App/Object/BaseService.cs
using App.utility;
using ConfApp;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using System.Linq.Expressions;

namespace App.Object
{
    /// <summary>
    /// سرویس پایه برای Business Logic - جدای از CrudService که فقط CRUD انجام می‌دهد
    /// </summary>
    public abstract class BaseService<TEntity> where TEntity : class
    {
        protected readonly MyContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseService(MyContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        // ========== ولیدیشن سریع ==========
        protected StatusResult ValidateModel<TModel>(TModel model)
            => ModelValidator.ValidateToStatusResult(model);

        protected async Task<StatusResult> ValidateUniqueAsync(
            Expression<Func<TEntity, bool>> predicate,
            string fieldName,
            string? value = null)
        {
            var exists = await _dbSet.AnyAsync(predicate);
            if (exists)
            {
                var message = string.IsNullOrEmpty(value)
                    ? MessageApp.DuplicateEntry
                    : MessageApp.DuplicateField(fieldName);
                return ResultFactory.Status(ResultStatusEnum.Conflict, message);
            }
            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        protected async Task<StatusResult> ValidateUniqueForUpdateAsync(
            Expression<Func<TEntity, bool>> predicate,
            int currentId,
            string fieldName,
            string value)
        {
            var param = Expression.Parameter(typeof(TEntity), "x");
            var idProperty = Expression.Property(param, "Id");
            var idEqual = Expression.NotEqual(idProperty, Expression.Constant(currentId));

            // ترکیب شرط‌ها
            var originalBody = predicate.Body;
            var combinedBody = Expression.AndAlso(originalBody, idEqual);
            var combinedPredicate = Expression.Lambda<Func<TEntity, bool>>(combinedBody, param);

            return await ValidateUniqueAsync(combinedPredicate, fieldName, value);
        }

        // ========== کوئری سریع با شرط IsDeleted ==========
        protected IQueryable<TEntity> GetActiveQuery()
        {
            // اگر TEntity از BaseDomain ارث برده باشد
            var baseDomainType = typeof(Domain.Objects.BaseDomain);
            if (baseDomainType.IsAssignableFrom(typeof(TEntity)))
            {
                var param = Expression.Parameter(typeof(TEntity), "x");
                var property = Expression.Property(param, "IsDeleted");
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda<Func<TEntity, bool>>(condition, param);
                return _dbSet.Where(lambda);
            }
            return _dbSet;
        }

        // ========== گرفتن Entity با بررسی NotFound ==========
        protected async Task<TEntity?> GetEntityOrNullAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetActiveQuery().FirstOrDefaultAsync(predicate);
        }

        protected async Task<TEntity> GetEntityOrThrowAsync(Expression<Func<TEntity, bool>> predicate, string entityName)
        {
            var entity = await GetEntityOrNullAsync(predicate);
            if (entity == null)
                throw new KeyNotFoundException(MessageApp.NotFoundItem(entityName));
            return entity;
        }

        // ========== گرفتن Entity با نتیجه StatusResult ==========
        protected async Task<SingleDataResult<TEntity>> GetEntityResultAsync(Expression<Func<TEntity, bool>> predicate, string entityName)
        {
            var entity = await GetEntityOrNullAsync(predicate);
            if (entity == null)
                return ResultFactory.Single<TEntity>(ResultStatusEnum.NotFound, null, MessageApp.NotFoundItem(entityName));
            return ResultFactory.Single(ResultStatusEnum.Success, entity);
        }

        // ========== Execute با مدیریت خطا ==========
        protected async Task<StatusResult> ExecuteAsync(Func<Task> action, string successMessage)
        {
            try
            {
                await action();
                await _context.SaveChangesAsync();
                return ResultFactory.Status(ResultStatusEnum.Success, successMessage);
            }
            catch (DbUpdateException ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                return ResultFactory.Status(ResultStatusEnum.DatabaseError, $"خطا در دیتابیس: {innerMsg}");
            }
            catch (Exception ex)
            {
                return ResultFactory.Status(ResultStatusEnum.InternalError, $"خطا: {ex.Message}");
            }
        }

        // ========== بررسی وجود ==========
        protected async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetActiveQuery().AnyAsync(predicate);
        }

        // ========== شمارش ==========
        protected async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = GetActiveQuery();
            if (predicate != null)
                query = query.Where(predicate);
            return await query.CountAsync();
        }
    }
}