using App.Contracts.Object.Shop.CountTypeCon;
using App.Object.Base;
using App.utility;
using AutoMapper;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Object.Shop.CountTypeApp
{
    public class CountTypeApp : CrudService<CountType, CountTypeView, CountTypeCreate, CountTypeView, int>,
                                ICountTypeApp
    {
        private readonly ICountTypeRep _rep;
        private readonly IMapper _mapper;

        public CountTypeApp(ICountTypeRep rep, IMapper mapper)
            : base(rep, mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }

        /*=== CRUD یک‌خطی ===*/
        public Task<ApiResult<List<CountTypeView>>> GetAll(Pagination pagination) => base.GetAllAsync(pagination);
        public Task<ApiResult<CountTypeView>> GetById(int id)                   => base.GetByIdAsync(id);
        public Task<ApiResult> Create(CountTypeCreate dto)                      => base.CreateAsync(dto);
        public Task<ApiResult> DeleteBy(List<int> ids)                          => base.DeleteAsync(ids);
        public Task<ApiResult> Update(CountTypeView dto)                        => base.UpdateAsync(dto);
    }

    public interface ICountTypeRep : IBaseRep<CountType, int> { }
}