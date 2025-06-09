using App.Contracts.Object.Shop.InvCon;
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
        public async Task<OPTResult<invCreate>> Create(invCreate createData)
        {
            var entity = _mapper.Map<Inv>(createData);
            await _rep.CreateAsync(entity);
            await _rep.SaveChangesAsync();

            var viewModel = _mapper.Map<invCreate>(entity);
            return OPTResult<invCreate>.Success(viewModel, "رکورد با موفقیت ایجاد شد.");
       
        }

        public async Task<OPTResult<invView>> DeleteBy(List<int> ids)
        {
             var entities = await _rep.GetByIdsAsync(ids);
            if (entities == null || entities.Count == 0)
                return OPTResult<invView>.Failed("هیچ رکوردی برای حذف یافت نشد.");

            var deletableEntities = new List<Inv>();
            var usedEntities = new List<Inv>();

            foreach (var entity in entities)
            {
                var isUsed = await _rep.HasRelationsAsync(entity); // ✅ بررسی استفاده در جداول رابطه‌ای
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

            return OPTResult<invView>.Success(message.Trim());
        }

        public async Task<OPTResult<invView>> GetAll(Pagination pagination)
        {
            var entities = await _rep.GetAsync(pagination);
            var viewModels = _mapper.Map<List<invView>>(entities);
            var totalRecords = await _rep.CountAsync();
            return new OPTResult<invView>
            {
                IsSucceeded = true,
                Message = "دریافت لیست با موفقیت انجام شد.",
                Data = viewModels,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalRecords = totalRecords,
                TotalPages = pagination.CalculateTotalPages(totalRecords)
            };

        }

        public async Task<OPTResult<invUpdate>> GetById(int id)
        {
            var entity = await _rep.GetAsync(id);
            if (entity == null)
                return OPTResult<invUpdate>.Failed("رکورد یافت نشد.");

            var viewModel = _mapper.Map<invUpdate>(entity);
            return OPTResult<invUpdate>.Success(viewModel, "رکورد با موفقیت دریافت شد.");
        }

        public async Task<OPTResult<invUpdate>> Update(invUpdate updateData)
        {
            var entity = await _rep.GetAsync(updateData.Id);
            if (entity == null)
                return OPTResult<invUpdate>.Failed("رکورد یافت نشد.");
            _mapper.Map(updateData, entity);
            await _rep.UpdateAsync(entity);
            await _rep.SaveChangesAsync();
            var viewModel = _mapper.Map<invUpdate>(entity);
            return OPTResult<invUpdate>.Success(viewModel, "رکورد با موفقیت به‌روزرسانی شد.");
        }
    }


    public interface IInvRep : IBaseRep<Inv, int> { }


    public class InvProfile : Profile
    {
        public InvProfile()
        {
            CreateMap<Inv, invView>();
            CreateMap<Inv, invUpdate>();
            CreateMap<invUpdate, Inv>();
            CreateMap<invCreate, Inv>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Inv, invCreate>();

        }
    }
}