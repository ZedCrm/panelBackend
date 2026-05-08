using API.Attributes;
using API.utility;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class GenericController<TDto, TCreate, TUpdate, TKey> : BaseController
        where TDto : class
        where TCreate : class
        where TUpdate : class
    {
        private readonly ICrudService<TDto, TCreate, TUpdate, TKey> _service;

        protected GenericController(ICrudService<TDto, TCreate, TUpdate, TKey> service)
        {
            _service = service;
        }

        [HttpPost("GetAll")]
        public virtual async Task<ActionResult<ListDataResult<TDto>>> GetAll([FromBody] Pagination pagination)
        {
            var result = await _service.GetAllAsync(pagination);
            return result;   // ✅ implicit conversion
        }

        [HttpGet("GetById")]
        public virtual async Task<ActionResult<SingleDataResult<TUpdate>>> GetById([FromQuery] TKey id)
        {
            var result = await _service.GetByIdAsync(id);
            return result;
        }

        [HttpPost("Create")]
        public virtual async Task<ActionResult<StatusResult>> Create([FromBody] TCreate dto)
        {
            var result = await _service.CreateAsync(dto);
            return result;
        }

        [HttpPost("Update")]
        public virtual async Task<ActionResult<StatusResult>> Update([FromBody] TUpdate dto)
        {
            var result = await _service.UpdateAsync(dto);
            return result;
        }

        [HttpPost("Delete")]
        public virtual async Task<ActionResult<StatusResult>> Delete([FromBody] List<TKey> ids)
        {
            var result = await _service.DeleteAsync(ids);
            return result;
        }
    }
}