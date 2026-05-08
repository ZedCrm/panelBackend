using API.Attributes;
using App.Contracts.Object.Shop.ProductCon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace API.Controllers.Shop
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : GenericController<ProductView, ProductCreate, ProductUpdate, int>
    {
        private readonly IProductApp _productApp;

        public ProductController(IProductApp productApp) : base(productApp)
        {
            _productApp = productApp;
        }

        [HttpPost("search")]
        public async Task<ActionResult<ListDataResult<ProductView>>> Search([FromBody] ProductSearchCriteria productSearch)
        {
            return await _productApp.SearchProducts(productSearch);
        }
    }
}