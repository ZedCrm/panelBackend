using App.Contracts.Object.Shop.CountTypeCon;
using App.Contracts.Object.Shop.ProductCon;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;

namespace API.Controllers.Shop
{

    public class CountTypeController : BaseController
    {
            private readonly ICountTypeApp countTypeApp;

           public CountTypeController(ICountTypeApp countTypeApp)
        {
            this.countTypeApp = countTypeApp;
        }
       

        [HttpPost]
        [Route("/api/CountType/GetAll")]
        public async Task<ActionResult<OPTResult<CountTypeView>>> Index([FromBody] Pagination pagination)
        {
             return await countTypeApp.GetAll(pagination);

             

            
        }
        [HttpGet]
        [Route("/api/CountType/GetById")]
        public async Task<ActionResult<OPTResult<CountTypeView>>> GetById([FromQuery] int id)
        {
            var result = await countTypeApp.GetById(id);
            if (result.IsSucceeded == true) { return Ok(result); }
            else { return Ok(result); }
        }
        


      


        [HttpPost]
        [Route("/api/CountType/create")]
        public async Task<ActionResult> create([FromBody] CountTypeCreate countTypeCreate)
        {

            var opt = await countTypeApp.Create(countTypeCreate);
            if (opt.IsSucceeded==true) { return Ok(opt); }
            else { return Ok ( opt); }
            

        }
       

        [HttpPost]
        [Route("/api/CountType/delete")]
        public async Task<ActionResult> delete([FromBody] List<int> ids)
        {
            var result = await countTypeApp.DeleteBy(ids); 
            return Ok(result);
        }

        [HttpDelete]
        [Route("/api/CountType/deletebyid")]
        public OkResult deletebyid([FromQuery] int id)
        {
            var ids = new List<int> { id };
            countTypeApp.DeleteBy(ids); // تغییر متد DeleteBy برای پذیرش لیست آی‌دی‌ها
            return Ok();
        }


        [HttpPost]
        [Route("/api/CountType/update")]
        public async Task<ActionResult> update([FromBody] CountTypeView countTypeView)
        {
            var opt = await countTypeApp.Update(countTypeView);
            if (opt.IsSucceeded == true) { return Ok(opt); }
            else { return Ok(opt); }  
        }        
        [HttpGet("/api/counttype/CountTypelist")]
        public IActionResult CountTypeList()
        {
            var pagination = new Pagination
            {
                PageNumber = 1,
                PageSize = 1000
            };
            var list = countTypeApp.GetAll(pagination);
            return Ok(list);
        }
       
    }
}
