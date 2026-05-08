using API.Attributes;
using App.Contracts.Object.Shop.InvCon;
using App.Contracts.Object.Shop.ProductCon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace API.Controllers.Shop
{
    /// <summary>
    /// کنترلر مربوط به مدیریت محصولات در سیستم
    /// </summary>
    [Authorize]
    public class ProductController :   GenericController<ProductView, ProductCreate, ProductUpdate, int>
    {
        private readonly IProductApp _productApp;

        public ProductController(IProductApp productApp) : base(productApp) 
        {
            _productApp = productApp;
        }

        /// <summary>
        /// جستجوی محصولات بر اساس معیارهای مشخص
        /// </summary>
        [HttpPost("/api/product/search")]
        public async Task<ActionResult<ListDataResult<ProductView>>> Search([FromBody] ProductSearchCriteria productSearch)
        {
            return await _productApp.SearchProducts(productSearch);
        }

    
    }
}