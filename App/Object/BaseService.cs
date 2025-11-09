// App\Object\BaseService.cs
using App.Object.Base.Users;
using App.utility;
using AutoMapper;
using Domain.Objects;
using MyFrameWork.AppTool;
using System.Linq.Expressions;

namespace App.Object
{
    public abstract class BaseService<TDto, TCreate, TUpdate, TEntity>
        where TDto : class
        where TCreate : class
        where TUpdate : class
        where TEntity : BaseDomain
    {
        protected readonly IBaseRep<TEntity, int> _repository;
        protected readonly IMapper _mapper;
        protected readonly IFileService? _fileService;
        protected readonly UserStatusService? _statusService;

        protected BaseService(
            IBaseRep<TEntity, int> repository,
            IMapper mapper,
            IFileService? fileService = null,
            UserStatusService? statusService = null)
        {
            _repository = repository;
            _mapper = mapper;
            _fileService = fileService;
            _statusService = statusService;
        }

        // GET ALL + Pagination
        public virtual async Task<ApiResult<List<TDto>>> GetAll(Pagination pagination)
        {
            var entities = await _repository.GetAsync(pagination);
            var dtos = _mapper.Map<List<TDto>>(entities);
            var total = await _repository.CountAsync();

            return ApiResult<List<TDto>>.PagedSuccess(dtos, total, pagination.PageNumber, pagination.PageSize);
        }

        // GET BY ID
        public virtual async Task<ApiResult<TUpdate>> GetById(int id)
        {
            var entity = await _repository.GetAsync(id);
            if (entity == null)
                return ApiResult<TUpdate>.Failed(MessageApp.NotFound, 404);

            var dto = _mapper.Map<TUpdate>(entity);
            return ApiResult<TUpdate>.Success(dto);
        }

        // CREATE
        public virtual async Task<ApiResult> Create(TCreate createDto)
        {
            var entity = _mapper.Map<TEntity>(createDto);

            await BeforeCreate(entity, createDto);
            await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
            await AfterCreate(entity, createDto);

            return ApiResult.Success("رکورد با موفقیت ایجاد شد.");
        }

        // UPDATE
        public virtual async Task<ApiResult> Update(TUpdate updateDto)
        {
            var idProp = updateDto.GetType().GetProperty("Id")?.GetValue(updateDto);
            if (idProp is not int id || id <= 0)
                return ApiResult.Failed("شناسه معتبر نیست.", 400);

            var entity = await _repository.GetAsync(id);
            if (entity == null)
                return ApiResult.Failed(MessageApp.NotFound, 404);

            _mapper.Map(updateDto, entity);

            await BeforeUpdate(entity, updateDto);
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            await AfterUpdate(entity, updateDto);

            return ApiResult.Success("رکورد با موفقیت به‌روزرسانی شد.");
        }

        // DELETE
        public virtual async Task<ApiResult> DeleteBy(List<int> ids)
        {
            if (!ids.Any())
                return ApiResult.Failed("هیچ شناسه‌ای انتخاب نشده است.", 400);

            var entities = await _repository.GetByIdsAsync(ids);
            if (!entities.Any())
                return ApiResult.Failed(MessageApp.NotFound, 404);

            foreach (var entity in entities)
                _repository.Delete(entity);

            await _repository.SaveChangesAsync();
            return ApiResult.Success("رکورد(ها) با موفقیت حذف شدند.");
        }

        // HOOKS — برای override در فرزند
        protected virtual Task BeforeCreate(TEntity entity, TCreate dto) => Task.CompletedTask;
        protected virtual Task AfterCreate(TEntity entity, TCreate dto) => Task.CompletedTask;
        protected virtual Task BeforeUpdate(TEntity entity, TUpdate dto) => Task.CompletedTask;
        protected virtual Task AfterUpdate(TEntity entity, TUpdate dto) => Task.CompletedTask;
    }
}