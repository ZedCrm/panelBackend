using System;
using System.Collections.Generic;
using API.Attributes;
using App.Contracts.Object.Shop.InvCon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;

namespace API.Controllers.Shop
{
    [Route("[controller]")]
    [Authorize]
    public class InvController : BaseController
    {
        private readonly IInvApp _invApp;
        public InvController(IInvApp invApp)
        {
            _invApp = invApp;
        }

        [HttpPost("/api/inv/GetAll")]
        [RequirePermission("ViewProduct")]
        public async Task<ActionResult<ApiResult<List<InvView>>>> Index([FromBody] Pagination pagination)
        {
            return await _invApp.GetAll(pagination);
        }

        [HttpGet("/api/inv/GetById")]
        [RequirePermission("InvView")]
        public async Task<ActionResult<ApiResult<InvUpdate>>> GetById([FromQuery] int id)
        {
            return await _invApp.GetById(id);
        }

        [HttpPost("/api/inv/create")]
        public async Task<ActionResult<ApiResult>> Create([FromBody] InvCreate invCreate)
        {
            return await _invApp.Create(invCreate);
        }

        [HttpPost("/api/inv/delete")]
        public async Task<ActionResult<ApiResult>> Delete([FromBody] List<int> ids)
        {
            return await _invApp.DeleteBy(ids);
        }

        [HttpDelete("/api/inv/deletebyid")]
        public async Task<ActionResult<ApiResult>> DeleteById([FromQuery] int id)
        {
            // متد DeleteBy لیستی می‌گیرد؛ یک آی‌دی را هم به صورت لیست ارسال می‌کنیم
            return await _invApp.DeleteBy(new List<int> { id });
        }

        [HttpPost("/api/inv/update")]
        public async Task<ActionResult<ApiResult>> Update([FromBody] InvUpdate invUpdate)
        {
            return await _invApp.Update(invUpdate);
        }
    }
}