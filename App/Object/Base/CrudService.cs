using App.utility;
using AutoMapper;
using Domain.Objects;
using MyFrameWork.AppTool;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Object.Base
{
    public abstract class CrudService<TEntity, TDto, TCreate, TUpdate, TKey>
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

        public virtual async Task<ApiResult<List<TDto>>> GetAllAsync(Pagination pagination)
        {
            var entities = await _repo.GetAsync(pagination);
            var data   = _mapper.Map<List<TDto>>(entities);
            var total  = await _repo.CountAsync();
            return ApiResult<List<TDto>>.PagedSuccess(data, total, pagination.PageNumber, pagination.PageSize);
        }

        public virtual async Task<ApiResult<TUpdate>> GetByIdAsync(TKey id)
        {
            var entity = await _repo.GetAsync(id);
            return entity == null
                ? ApiResult<TUpdate>.Failed(MessageApp.NotFound, 404)
                : ApiResult<TUpdate>.Success(_mapper.Map<TUpdate>(entity));
        }

        public virtual async Task<ApiResult> CreateAsync(TCreate dto)
        {
            var validation = ModelValidator.ValidateToOpt(dto);
            if (!validation.IsSucceeded) return ApiResult.Failed(validation.Message);

            var entity = _mapper.Map<TEntity>(dto);
            await _repo.CreateAsync(entity);
            await _repo.SaveChangesAsync();
            return ApiResult.Success(MessageApp.CustomSuccess("افزودن"));
        }

        public virtual async Task<ApiResult> UpdateAsync(TUpdate dto)
        {
            var validation = ModelValidator.ValidateToOpt(dto);
            if (!validation.IsSucceeded) return ApiResult.Failed(validation.Message);

            var id = (TKey)typeof(TUpdate).GetProperty("Id")!.GetValue(dto)!;
            var entity = await _repo.GetAsync(id);
            if (entity == null) return ApiResult.Failed(MessageApp.NotFound, 404);

            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return ApiResult.Success(MessageApp.CustomSuccess("ویرایش"));
        }

        public virtual async Task<ApiResult> DeleteAsync(List<TKey> ids)
        {
            if (ids == null || !ids.Any()) return ApiResult.Failed("هیچ شناسه‌ای ارسال نشده است.");

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

            return ApiResult.Success(msg.Trim());
        }
    }
}