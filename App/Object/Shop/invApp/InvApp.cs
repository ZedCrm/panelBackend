using App.Contracts.Object.Shop.InvCon;
using App.utility;
using AutoMapper;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;

namespace App.Object.Shop.invApp
{
    public class InvApp : IInvApp
    {

        private readonly IInvRep _rep;
        private readonly IMapper _mapper;
        public InvApp(IInvRep rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        



        public async Task<OPTResult<InvCreate>> Create(InvCreate createData)
        {
            var validateAllProperties =  ModelValidator.ValidateToOptResult<InvCreate>(createData);
            if (!validateAllProperties.IsSucceeded) return validateAllProperties;

            var entity = _mapper.Map<Inv>(createData);
            await _rep.CreateAsync(entity);
            await _rep.SaveChangesAsync();

            var viewModel = _mapper.Map<InvCreate>(entity);
            return OPTResult<InvCreate>.Success(viewModel, MessageApp.AcceptOpt);

        }




        public async Task<OPTResult<InvView>> DeleteBy(List<int> ids)
        {
            var entities = await _rep.GetByIdsAsync(ids);
            if (entities == null || entities.Count == 0)
                return OPTResult<InvView>.Failed(MessageApp.NotFound);

            var deletableEntities = new List<Inv>();
            var usedEntities = new List<Inv>();

            foreach (var entity in entities)
            {
                var isUsed = await _rep.HasRelationsAsync(entity); 
                if (!isUsed)
                    deletableEntities.Add(entity);
                else
                    usedEntities.Add(entity);
            }

            if (deletableEntities.Any())
            {
                _rep.DeleteRange(deletableEntities);
                await _rep.SaveChangesAsync();
            }

            string message = "";
            if (deletableEntities.Count > 0)
                message += $"{deletableEntities.Count} رکورد با موفقیت حذف شد. ";
            if (usedEntities.Count > 0)
                message += $"{usedEntities.Count} مورد به دلیل استفاده در بخش‌های دیگر حذف نشد.";

            return OPTResult<InvView>.Success(message.Trim());
        }






        public async Task<OPTResult<InvView>> GetAll(Pagination pagination)
        {
            var entities = await _rep.GetAsync(pagination);
            var viewModels = _mapper.Map<List<InvView>>(entities);
            var totalRecords = await _rep.CountAsync();
            return new OPTResult<InvView>
            {
                IsSucceeded = true,
                Message = MessageApp.AcceptOpt,
                Data = viewModels,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalRecords = totalRecords,
                TotalPages = pagination.CalculateTotalPages(totalRecords)
            };

        }






        public async Task<OPTResult<InvUpdate>> GetById(int id)
        {
            var entity = await _rep.GetAsync(id);
            if (entity == null)
                return OPTResult<InvUpdate>.Failed(MessageApp.FailOpt);

            var viewModel = _mapper.Map<InvUpdate>(entity);
            return OPTResult<InvUpdate>.Success(viewModel, MessageApp.AcceptOpt);
        }







        public async Task<OPTResult<InvUpdate>> Update(InvUpdate updateData)
        {
            var entity = await _rep.GetAsync(updateData.Id);
            if (entity == null)
                return OPTResult<InvUpdate>.Failed(MessageApp.NotFound);
            _mapper.Map(updateData, entity);
            await _rep.UpdateAsync(entity);
            await _rep.SaveChangesAsync();
            var viewModel = _mapper.Map<InvUpdate>(entity);
            return OPTResult<InvUpdate>.Success(viewModel,MessageApp.AcceptOpt);
        }
    }


 



    public interface IInvRep : IBaseRep<Inv, int> { }



}