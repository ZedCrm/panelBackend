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
        [Route("/GetAll")]
        public async Task<ActionResult<OPTResult<ProductView>>> Index([FromBody] Pagination pagination)
        {
             return await productApp.GetAll(pagination);

             

            
        }

        [HttpPost]
        [Route("/search")]
        public async Task<ActionResult<IEnumerable<ProductView>>> search([FromBody]  ProductSearchCriteria productSearch
            )
        {
            
            return await productApp.SearchProducts(productSearch);
            ;
        }



        [HttpPost]
        [Route("/creat")]
        public async Task<ActionResult> create([FromBody] ProductCreate product)
        {

            var opt = await productApp.Create(product);
            if (opt.IsSucceeded) { return Ok(); }
            else { return Ok ( new { warning = opt.Message } ); }
            

        }

        [HttpDelete]
        [Route("/delete")]
        public OkResult delete([FromBody] List<int> ids)
        {
            productApp.DeleteBy(ids); // تغییر متد DeleteBy برای پذیرش لیست آی‌دی‌ها
            return Ok();
        }
    }
}
