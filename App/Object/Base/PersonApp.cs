using App.Contracts.Object.Base;
using App.Contracts.Object.Shop.ProductCon;
using AutoMapper;
using Domain.Objects.Base;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Base
{
    public class PersonApp : CrudService<Person, PersonView, PersonCreate, PersonUpdate, int>, IPersonApp
    {
        private readonly IPersonRep _ctx;
        private readonly IMapper mapper;

        public PersonApp(IPersonRep personRep , IMapper mapper) : base(personRep ,mapper)
        {
            _ctx = personRep;
            this.mapper = mapper;
        }

        public Task<ListDataResult<PersonView>> GetAll(Pagination pagination) => base.GetAllAsync(pagination);
        public Task<SingleDataResult<PersonUpdate>> GetById(int id) => base.GetByIdAsync(id);
        public Task<StatusResult> Create(PersonCreate dto) => base.CreateAsync(dto);
        public Task<StatusResult> DeleteBy(List<int> ids) => base.DeleteAsync(ids);
        public Task<StatusResult> Update(PersonUpdate dto) => base.UpdateAsync(dto);


    }


    public interface IPersonRep : IBaseRep<Person, int>
    {

    }
}
