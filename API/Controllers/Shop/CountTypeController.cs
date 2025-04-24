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

      


        [HttpPost]
        [Route("/api/CountType/create")]
        public async Task<ActionResult> create([FromBody] CountTypeCreate countTypeCreate)
        {

            var opt = await countTypeApp.Create(countTypeCreate);
            if (opt.IsSucceeded==true) { return Ok(); }
            else { return Ok ( new { warning = opt.Message } ); }
            

        }
       


       
    }
}
