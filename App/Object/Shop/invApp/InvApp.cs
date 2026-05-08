using App.Contracts.Object.Shop.InvCon;
using App.utility;
using AutoMapper;
using ConfApp;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Shop.InvApp
{
    public class InvApp : CrudService<Inv, InvView, InvCreate, InvUpdate, int>,
                          IInvApp
    {
     

        public InvApp(MyContext ctx, IMapper mapper)
            : base(ctx, mapper)
        {
            
        }

        public Task<ListDataResult<InvView>> GetAll(Pagination pagination) => base.GetAllAsync(pagination);
        public Task<SingleDataResult<InvUpdate>> GetById(int id)                 => base.GetByIdAsync(id);
        public Task<StatusResult> Create(InvCreate dto)                      => base.CreateAsync(dto);
        public Task<StatusResult> DeleteBy(List<int> ids)                    => base.DeleteAsync(ids);
        public Task<StatusResult> Update(InvUpdate dto)                      => base.UpdateAsync(dto);
    }

   
}