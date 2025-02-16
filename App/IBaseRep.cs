using App.Contracts.Object.Shop.ProductCon;
using Domain.Objects;
using MyFrameWork.AppTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    public interface IBaseRep<T, TKey> : IDisposable where T : BaseDomain
    {
        Task<T> GetAsync(TKey id);
        T Get(TKey id);
        Task<List<T>>  GetFilteredAsync(Expression<Func<T, bool>> filter = null , ProductSearchCriteria criteria = null);
        Task<List<T>> GetAsync();
        Task<List<T>> GetAsync(Pagination pagination );
        Task CreateAsync(T entity);
        void Delete(T entity);
        void DeleteById(TKey id);
        Task<int> CountAsync();
        Task<bool> ExistAsync(Expression<Func<T, bool>> expression  );
        Task SaveChangesAsync();
    }
}
