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
    public class ProductController : BaseController
    {
        private readonly IProductApp _productApp;

        /// <summary>
        /// سازنده کنترلر، تزریق وابستگی IProductApp
        /// </summary>
        /// <param name="productApp">سرویس لایه اپلیکیشن برای مدیریت محصولات</param>
        public ProductController(IProductApp productApp)
        {
            _productApp = productApp;
        }

        /// <summary>
        /// دریافت لیست محصولات با صفحه‌بندی
        /// </summary>
        /// <param name="pagination">اطلاعات صفحه‌بندی</param>
        /// <returns>لیست محصولات یا خطای عدم دسترسی</returns>
        [HttpPost]
        [Route("/api/product/GetAll")]
        [RequirePermission("ViewProduct")]
        public async Task<ActionResult<OPTResult<ProductView>>> Index([FromBody] Pagination pagination)
        {
           
            // فراخوانی متد GetAll از لایه اپلیکیشن با userId
            return await _productApp.GetAll(pagination);
        }

        /// <summary>
        /// جستجوی محصولات بر اساس معیارهای مشخص
        /// </summary>
        /// <param name="productSearch">معیارهای جستجو</param>
        /// <returns>لیست محصولات فیلترشده</returns>
        [HttpPost]
        [Route("/api/product/search")]
        public async Task<ActionResult<OPTResult<ProductView>>> Search([FromBody] ProductSearchCriteria productSearch)
        {
            return await _productApp.SearchProducts(productSearch);
        }

        /// <summary>
        /// ایجاد محصول جدید
        /// </summary>
        /// <param name="product">اطلاعات محصول جدید</param>
        /// <returns>نتیجه عملیات ایجاد</returns>
        [HttpPost]
        [Route("/api/product/create")]
        public async Task<ActionResult> Create([FromBody] ProductCreate product)
        {
            var opt = await _productApp.Create(product);
            return Ok(opt);
        }

        /// <summary>
        /// حذف محصولات بر اساس لیست آیدی‌ها
        /// </summary>
        /// <param name="ids">لیست آیدی‌های محصولات</param>
        /// <returns>نتیجه عملیات حذف</returns>
        [HttpPost]
        [Route("/api/product/delete")]
        public async Task<OkObjectResult> Delete([FromBody] List<int> ids)
        {
            var opt = await _productApp.DeleteBy(ids);
            return Ok(opt);
        }

        /// <summary>
        /// دریافت اطلاعات یک محصول بر اساس آیدی
        /// </summary>
        /// <param name="id">آیدی محصول</param>
        /// <returns>اطلاعات محصول یا پیام خطا</returns>
        [HttpGet]
        [Route("/api/product/GetById")]
        public async Task<ActionResult<OPTResult<ProductUpdate>>> GetById([FromQuery] int id)
        {
            var result = await _productApp.GetById(id);
            if (result.IsSucceeded)
            {
                return Ok(result);
            }
            return Ok(new { warning = result.Message });
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات محصول
        /// </summary>
        /// <param name="product">اطلاعات به‌روزرسانی‌شده محصول</param>
        /// <returns>نتیجه عملیات به‌روزرسانی</returns>
        [HttpPost]
        [Route("/api/product/update")]
        public async Task<ActionResult> Update([FromBody] ProductView product)
        {
            var opt = await _productApp.Update(product);
            if (opt.IsSucceeded)
            {
                return Ok(opt);
            }
            return Ok(new { warning = opt.Message });
        }
    }
}