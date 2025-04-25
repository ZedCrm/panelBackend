using App.Contracts.Object.Shop.ProductCon;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;

namespace API.Controllers.Shop
{

    public class Product : BaseController
    {
        private readonly IProductApp productApp;

        public Product(IProductApp productApp)
        {
            this.productApp = productApp;
        }


        [HttpPost]
        [Route("/api/product/GetAll")]
        public async Task<ActionResult<OPTResult<ProductView>>> Index([FromBody] Pagination pagination)
        {
             return await productApp.GetAll(pagination);

             

            
        }

        [HttpPost]
        [Route("/api/product/search")]
        public async Task<ActionResult<IEnumerable<ProductView>>> search([FromBody]  ProductSearchCriteria productSearch
            )
        {
            
            return await productApp.SearchProducts(productSearch);
            ;
        }



        [HttpPost]
        [Route("/api/product/create")]
        public async Task<ActionResult> create([FromBody] ProductCreate product)
        {

            var opt = await productApp.Create(product);
            if (opt.IsSucceeded) { return Ok(opt); }
            else { return Ok ( new { warning = opt.Message } ); }
            

        }

        [HttpPost]
        [Route("/api/product/delete")]
        public OkResult delete([FromBody] List<int> ids)
        {
            productApp.DeleteBy(ids); // تغییر متد DeleteBy برای پذیرش لیست آی‌دی‌ها
            return Ok();
        }

        [Route("/api/product/GetById")]
        public async Task<ActionResult<OPTResult<ProductView>>> GetById([FromQuery] int id)
        {
            var result = await productApp.GetById(id);
            if (result.IsSucceeded == true) { return Ok(result); }
            else { return Ok(new { warning = result.Message }); }
        }

        /** update */
        [HttpPost]
        [Route("/api/product/update")]
        public async Task<ActionResult> update([FromBody] ProductView product)
        {
            var opt = await productApp.Update(product);
            if (opt.IsSucceeded==true) { return Ok(opt); }
            else { return Ok(new { warning = opt.Message }); }
            
        }


  
    }
}
