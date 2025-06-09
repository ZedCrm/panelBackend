using App;
using App.Contracts.Object.Shop.ProductCon;
using Domain.Objects;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfApp

{
    public abstract class BaseRep<T, TKey> : IBaseRep<T, TKey> where T : BaseDomain
    {
        #region constructor
        private readonly MyContext _ctx;
        public BaseRep(MyContext ctx)
        {
            _ctx = ctx;
        }
        #endregion




        public async Task<int> CountAsync()
        {
            return await _ctx.Set<T>().Where(e => !e.IsDeleted).CountAsync();
        }

     public async Task<bool> HasRelationsAsync(T entity)
{
    var entityType = _ctx.Model.FindEntityType(typeof(T));
    if (entityType == null)
        throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in DbContext model.");

    var primaryKey = entityType.FindPrimaryKey();
    if (primaryKey == null)
        throw new InvalidOperationException($"Primary key for entity {typeof(T).Name} not found.");

    var entityKey = primaryKey.Properties.FirstOrDefault();
    if (entityKey == null)
        throw new InvalidOperationException($"Primary key property not found for entity {typeof(T).Name}.");

    var entityKeyValue = entity.GetType().GetProperty(entityKey.Name)?.GetValue(entity);
    if (entityKeyValue == null)
        throw new InvalidOperationException($"Value of primary key property '{entityKey.Name}' is null.");

    var foreignKeys = _ctx.Model.GetEntityTypes()
        .SelectMany(t => t.GetForeignKeys())
        .Where(fk => fk.PrincipalEntityType.ClrType == typeof(T));

    foreach (var fk in foreignKeys)
    {
        var dependentEntityType = fk.DeclaringEntityType.ClrType;

        var dbSet = _ctx.GetType().GetMethod("Set", Type.EmptyTypes)
            ?.MakeGenericMethod(dependentEntityType)
            .Invoke(_ctx, null) as IQueryable;

        if (dbSet == null)
            continue;

        var param = Expression.Parameter(dependentEntityType, "x");
        var fkProperty = fk.Properties.FirstOrDefault();
        if (fkProperty == null)
            continue;

        var property = Expression.Property(param, fkProperty.Name);
        var constant = Expression.Constant(entityKeyValue);
        var equal = Expression.Equal(property, constant);

        var lambdaType = typeof(Func<,>).MakeGenericType(dependentEntityType, typeof(bool));
        var lambda = Expression.Lambda(lambdaType, equal, param);

        // متد AnyAsync با ۳ پارامتر (شامل CancellationToken)
        var method = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "AnyAsync" && m.GetParameters().Length == 3);

        if (method == null)
            throw new InvalidOperationException("متد AnyAsync با پارامترهای سه‌گانه یافت نشد.");

        method = method.MakeGenericMethod(dependentEntityType);

        var task = (Task<bool>)method.Invoke(null, new object[] { dbSet, lambda, CancellationToken.None });

        if (await task)
            return true;
    }

    return false;
}



        public async Task CreateAsync(T entity)
        {
            entity.CreateDate = DateTime.Now;
            await _ctx.AddAsync(entity);
        }



        public void Delete(T entity)
        {
            entity.UpdateDate = DateTime.Now;
            entity.IsDeleted = true;
            _ctx.Update(entity);
        }



        public void DeleteById(TKey id)
        {
            var TforDelete = this.Get(id);

            if (TforDelete == null)
            {
                Console.WriteLine($"No item found with ID: {id}");
                return; // یا می‌توانید استثنایی پرتاب کنید  
            }
            this.Delete(TforDelete);
        }



        public async Task<bool> ExistAsync(Expression<Func<T, bool>> expression)
        {
            return await _ctx.Set<T>().AnyAsync(expression);
        }

        public async Task<T> GetAsync(TKey id)
        {


            return await _ctx.Set<T>().FindAsync(id);


        }

        public async Task<List<T>> GetAsync()
        {
            return await _ctx.Set<T>().Where(e => !e.IsDeleted).AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> GetAsync(Pagination pagination)
        {
            var query = _ctx.Set<T>().Where(e => !e.IsDeleted).AsQueryable();

            // Sorting  
            if (!string.IsNullOrEmpty(pagination.SortBy))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, pagination.SortBy);
                var orderByExpression = Expression.Lambda(property, parameter);

                // Use dynamic methods to infer type arguments  
                var methodName = pagination.SortDirection ? "OrderBy" : "OrderByDescending";
                var method = typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.Type);

                // Call the method dynamically  
                query = (IQueryable<T>)method.Invoke(null, new object[] { query, orderByExpression });
            }

            // Apply pagination  
            return await query.Skip(pagination.CalculateSkip())
                        .Take(pagination.PageSize).AsNoTracking()
                        .ToListAsync();
        }







        public async Task<List<T>> GetFilteredAsync(Expression<Func<T, bool>> filter = null, ProductSearchCriteria pagination = null)
        {

            IQueryable<T> query = _ctx.Set<T>().Where(e => !e.IsDeleted);

            // Apply filter if provided  
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply sorting if pagination is provided  
            if (pagination != null)
            {
                // Create an expression for sorting  
                if (!string.IsNullOrEmpty(pagination.SortBy))
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.Property(parameter, pagination.SortBy);
                    var orderByExpression = Expression.Lambda(property, parameter);

                    // Select the appropriate OrderBy or OrderByDescending method  
                    var methodName = pagination.SortDirection ? "OrderBy" : "OrderByDescending";
                    var method = typeof(Queryable)
                        .GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(T), property.Type);

                    // Call the method dynamically  
                    query = (IQueryable<T>)method.Invoke(null, new object[] { query, orderByExpression });
                }

                // Apply pagination  
                query = query.Skip(pagination.CalculateSkip())
                             .Take(pagination.PageSize);
            }

            return await query.AsNoTracking().ToListAsync();
        }






        public async Task SaveChangesAsync()
        {
            await _ctx.SaveChangesAsync();
        }

        public void Dispose()
        {
            Console.WriteLine($"MyContext disposed at {DateTime.Now}");
            _ctx?.Dispose();
        }

        public T Get(TKey id)
        {
            return _ctx.Set<T>().Find(id);
        }

        public Task DeleteByIDAsinc(TKey id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
        {
            return await _ctx.Set<T>().Where(e => !e.IsDeleted).Where(filter).CountAsync();
        }


        public Task<List<T>> GetByIdsAsync(List<TKey> ids)
        {
            return _ctx.Set<T>().Where(e => ids.Contains((TKey)(object)e.Id)).ToListAsync();
        }
        public void DeleteRange(List<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdateDate = DateTime.Now;
                entity.IsDeleted = true;
            }
            _ctx.UpdateRange(entities);
        }

        public Task<bool> UpdateAsync(T entity)
        {
            entity.UpdateDate = DateTime.Now;
            _ctx.Update(entity);
            return Task.FromResult(true);

        }
    }
}
