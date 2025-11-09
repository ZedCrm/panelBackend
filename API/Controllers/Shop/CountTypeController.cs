using App.Contracts.Object.Shop.CountTypeCon;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.Shop
{
    public class CountTypeController : BaseController
    {
        private readonly ICountTypeApp _countTypeApp;

        public CountTypeController(ICountTypeApp countTypeApp)
        {
            _countTypeApp = countTypeApp;
        }

        [HttpPost("/api/CountType/GetAll")]
        public async Task<ActionResult<ApiResult<List<CountTypeView>>>> Index([FromBody] Pagination pagination)
        {
            return await _countTypeApp.GetAll(pagination);
        }

        [HttpGet("/api/CountType/GetById")]
        public async Task<ActionResult<ApiResult<CountTypeView>>> GetById([FromQuery] int id)
        {
            return await _countTypeApp.GetById(id);
        }

        [HttpPost("/api/CountType/create")]
        public async Task<ActionResult<ApiResult>> Create([FromBody] CountTypeCreate dto)
        {
            return await _countTypeApp.Create(dto);
        }

        [HttpPost("/api/CountType/delete")]
        public async Task<ActionResult<ApiResult>> Delete([FromBody] List<int> ids)
        {
            return await _countTypeApp.DeleteBy(ids);
        }

        [HttpDelete("/api/CountType/deletebyid")]
        public async Task<ActionResult<ApiResult>> DeleteById([FromQuery] int id)
        {
            return await _countTypeApp.DeleteBy(new List<int> { id });
        }

        [HttpPost("/api/CountType/update")]
        public async Task<ActionResult<ApiResult>> Update([FromBody] CountTypeView dto)
        {
            return await _countTypeApp.Update(dto);
        }

        [HttpGet("/api/counttype/CountTypelist")]
        public async Task<ActionResult<ApiResult<List<CountTypeView>>>> CountTypeList()
        {
            var pagination = new Pagination { PageNumber = 1, PageSize = 1000 };
            return await _countTypeApp.GetAll(pagination);
        }
    }
}