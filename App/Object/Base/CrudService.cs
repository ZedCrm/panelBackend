using App.utility;
using AutoMapper;
using Domain.Objects;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Base
{
    public abstract class CrudService<TEntity, TDto, TCreate, TUpdate, TKey> : ICrudService<TDto, TCreate, TUpdate, TKey>
        where TEntity : BaseDomain
        where TDto : class
        where TCreate : class
        where TUpdate : class
    {
        protected readonly IBaseRep<TEntity, TKey> _repo;
        protected readonly IMapper _mapper;

        protected CrudService(IBaseRep<TEntity, TKey> repository, IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }

        public virtual async Task<ListDataResult<TDto>> GetAllAsync(Pagination pagination)
        {
            var entities = await _repo.GetAsync(pagination);
            var data   = _mapper.Map<List<TDto>>(entities);
            var total  = await _repo.CountAsync();
            return ResultFactory.List<TDto>(ResultStatusEnum.Success ,data ,total, pagination );
        }

        public virtual async Task<SingleDataResult<TUpdate>> GetByIdAsync(TKey id)
        {
            var entity = await _repo.GetAsync(id);
            return entity == null
                ? ResultFactory.Single<TUpdate>(ResultStatusEnum.NotFound,null ,MessageApp.NotFound)
                : ResultFactory.Single(ResultStatusEnum.Success,_mapper.Map<TUpdate>(entity));
        }

        public virtual async Task<StatusResult> CreateAsync(TCreate dto)
        {
            var validation = ModelValidator.ValidateToStatusResult(dto);
            if (!validation.IsSuccess) return validation;

            var entity = _mapper.Map<TEntity>(dto);
            await _repo.CreateAsync(entity);
            await _repo.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Success,MessageApp.CustomSuccess("افزودن"));
        }

        public virtual async Task<StatusResult> UpdateAsync(TUpdate dto)
        {
            var validation = ModelValidator.ValidateToStatusResult(dto);
            if (!validation.IsSuccess) return validation;

            var id = (TKey)typeof(TUpdate).GetProperty("Id")!.GetValue(dto)!;
            var entity = await _repo.GetAsync(id);
            if (entity == null) return ResultFactory.Status(ResultStatusEnum.NotFound,MessageApp.NotFound);

            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Success ,MessageApp.CustomSuccess("ویرایش"));
        }

        public virtual async Task<StatusResult> DeleteAsync(List<TKey> ids)
        {
            if (ids == null || !ids.Any()) return ResultFactory.Status(ResultStatusEnum.BadRequest,"هیچ شناسه‌ای ارسال نشده است.");

            var entities = await _repo.GetByIdsAsync(ids);
            var deletable = new List<TEntity>();
            var used = new List<TEntity>();

            foreach (var e in entities)
            {
                var hasRel = await _repo.HasRelationsAsync(e);
                (hasRel ? used : deletable).Add(e);
            }

            if (deletable.Any())
            {
                _repo.DeleteRange(deletable);
                await _repo.SaveChangesAsync();
            }

            var msg = "";
            if (deletable.Count > 0) msg += $"{deletable.Count} رکورد حذف شد. ";
            if (used.Count > 0) msg += $"{used.Count} مورد به دلیل استفاده حذف نشد.";

            return ResultFactory.Status(ResultStatusEnum.Success,msg.Trim());
        }
    }
}