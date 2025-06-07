using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using App.Contracts.Object.Shop.InvCon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFrameWork.AppTool;

namespace API.Controllers.Shop
{
    [Route("[controller]")]
    public class InvController : BaseController
    {


        private readonly IInvApp invApp;
        private readonly ILogger<InvController> _logger;
        public InvController(IInvApp invApp, ILogger<InvController> logger)
        {
            this.invApp = invApp;
            _logger = logger;

        }



        [HttpPost]
        [Route("/api/inv/GetAll")]
        public async Task<ActionResult<OPTResult<invView>>> Index([FromBody] Pagination pagination)
        {
            return await invApp.GetAll(pagination);
        }

        [HttpGet]
        [Route("/api/inv/GetById")]
        public async Task<ActionResult<OPTResult<invUpdate>>> GetById([FromQuery] int id)
        {
            var result = await invApp.GetById(id);
            if (result.IsSucceeded == true) { return Ok(result); }
            else { return Ok(result); }
        }

        [HttpPost]
        [Route("/api/inv/create")]
        public async Task<ActionResult> create([FromBody] invCreate invCreate)
        {

            var opt = await invApp.Create(invCreate);
            if (opt.IsSucceeded == true) { return Ok(opt); }
            else { return Ok(opt); }


        }

        [HttpPost]
        [Route("/api/inv/delete")]
        public async Task<ActionResult> delete([FromBody] List<int> ids)
        {
            var result = await invApp.DeleteBy(ids);
            return Ok(result);
        }

        [HttpDelete]
        [Route("/api/inv/deletebyid")]
        public OkResult deletebyid([FromQuery] int id)
        {
            var ids = new List<int> { id };
            invApp.DeleteBy(ids); // تغییر متد DeleteBy برای پذیرش لیست آی‌دی‌ها
            return Ok();
        }


        [HttpPost]
        [Route("/api/inv/update")]
        public async Task<ActionResult> update([FromBody] invUpdate invUpdate)
        {
            var opt = await invApp.Update(invUpdate);
            if (opt.IsSucceeded == true) { return Ok(opt); }
            else { return Ok(opt); }  
        }  


    }
}