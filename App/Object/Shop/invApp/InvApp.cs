using App.Contracts.Object.Shop.InvCon;
using App.Object.Base;
using App.utility;
using AutoMapper;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;

namespace App.Object.Shop.InvApp
{
    public class InvApp : CrudService<Inv, InvView, InvCreate, InvUpdate, int>,
                          IInvApp
    {
        private readonly IInvRep _invRep;
        private readonly IMapper _mapper;

        public InvApp(IInvRep invRep, IMapper mapper)
            : base(invRep, mapper)
        {
            _invRep = invRep;
            _mapper = mapper;
        }

        public Task<ApiResult<List<InvView>>> GetAll(Pagination pagination) => base.GetAllAsync(pagination);
        public Task<ApiResult<InvUpdate>> GetById(int id)                 => base.GetByIdAsync(id);
        public Task<ApiResult> Create(InvCreate dto)                      => base.CreateAsync(dto);
        public Task<ApiResult> DeleteBy(List<int> ids)                    => base.DeleteAsync(ids);
        public Task<ApiResult> Update(InvUpdate dto)                      => base.UpdateAsync(dto);
    }

    public interface IInvRep : IBaseRep<Inv, int> { }
}