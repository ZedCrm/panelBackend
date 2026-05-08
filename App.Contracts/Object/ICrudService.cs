using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

public interface ICrudService<TDto, TCreate, TUpdate, TKey>
    where TDto : class
    where TCreate : class
    where TUpdate : class
{
    Task<ListDataResult<TDto>> GetAllAsync(Pagination pagination);
    Task<SingleDataResult<TUpdate>> GetByIdAsync(TKey id);
    Task<StatusResult> CreateAsync(TCreate dto);
    Task<StatusResult> UpdateAsync(TUpdate dto);
    Task<StatusResult> DeleteAsync(List<TKey> ids);
}