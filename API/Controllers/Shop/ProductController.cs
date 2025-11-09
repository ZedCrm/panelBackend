using API.Attributes;
using App.Contracts.Object.Shop.ProductCon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;

namespace API.Controllers.Shop
{
    /// <summary>
    /// کنترلر مربوط به مدیریت محصولات در سیستم
    /// </summary>
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductApp _productApp;

        public ProductController(IProductApp productApp)
        {
            _productApp = productApp;
        }

        /// <summary>
        /// دریافت لیست محصولات با صفحه‌بندی
        /// </summary>
        [RequirePermission("ViewProduct")]
        [HttpPost("/api/product/GetAll")]
        public async Task<ActionResult<ApiResult<List<ProductView>>>> Index([FromBody] Pagination pagination)
        {
            return await _productApp.GetAll(pagination);
        }

        /// <summary>
        /// جستجوی محصولات بر اساس معیارهای مشخص
        /// </summary>
        [HttpPost("/api/product/search")]
        public async Task<ActionResult<ApiResult<List<ProductView>>>> Search([FromBody] ProductSearchCriteria productSearch)
        {
            return await _productApp.SearchProducts(productSearch);
        }

        /// <summary>
        /// ایجاد محصول جدید
        /// </summary>
        [HttpPost("/api/product/create")]
        public async Task<ActionResult<ApiResult>> Create([FromBody] ProductCreate product)
        {
            return await _productApp.Create(product);
        }

        /// <summary>
        /// حذف محصولات بر اساس لیست آیدی‌ها
        /// </summary>
        [HttpPost("/api/product/delete")]
        public async Task<ActionResult<ApiResult>> Delete([FromBody] List<int> ids)
        {
            return await _productApp.DeleteBy(ids);
        }

        /// <summary>
        /// دریافت اطلاعات یک محصول بر اساس آیدی
        /// </summary>
        [HttpGet("/api/product/GetById")]
        public async Task<ActionResult<ApiResult<ProductUpdate>>> GetById([FromQuery] int id)
        {
            return await _productApp.GetById(id);
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات محصول
        /// </summary>
        [HttpPost("/api/product/update")]
        public async Task<ActionResult<ApiResult>> Update([FromBody] ProductUpdate product)
        {
            return await _productApp.Update(product);
        }
    }
}