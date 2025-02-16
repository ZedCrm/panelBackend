using App;
using App.Contracts.Object.Shop.ProductCon;
using Domain.Objects;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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




        public async Task CreateAsync(T entity)
        {
            entity.CreateDate = DateTime.Now;
            await _ctx.AddAsync(entity);
        }



        public  void Delete(T entity)
        {
            entity.UpdateDate = DateTime.Now;
            entity.IsDeleted = true;
             _ctx.Update(entity);
        }


        
        public void DeleteById(TKey id)
        {
            var TforDelete =  this.Get(id);

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

        public async  Task<List<T>> GetAsync()
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

            return  await query.AsNoTracking().ToListAsync();
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
    }
}
