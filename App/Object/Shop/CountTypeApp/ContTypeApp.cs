using App.Contracts.Object.Shop.CountTypeCon;
using App.Object.Base;
using App.utility;
using AutoMapper;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
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
        public Task<ListDataResult<CountTypeView>> GetAll(Pagination pagination) => base.GetAllAsync(pagination);
        public Task<SingleDataResult<CountTypeView>> GetById(int id)                   => base.GetByIdAsync(id);
        public Task<StatusResult> Create(CountTypeCreate dto)                      => base.CreateAsync(dto);
        public Task<StatusResult> DeleteBy(List<int> ids)                          => base.DeleteAsync(ids);
        public Task<StatusResult> Update(CountTypeView dto)                        => base.UpdateAsync(dto);
    }

    public interface ICountTypeRep : IBaseRep<CountType, int> { }
}