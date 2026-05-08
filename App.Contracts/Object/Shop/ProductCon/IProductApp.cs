using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;


namespace App.Contracts.Object.Shop.ProductCon
{
        public interface IProductApp   : ICrudService<ProductView, ProductCreate, ProductUpdate, int>
    {
        /// <summary>
    /// جستجوی محصولات بر اساس معیارهای مشخص (نام، حداقل و حداکثر قیمت).
    /// </summary>
    /// <param name="criteria">معیارهای جستجو شامل نام، قیمت و اطلاعات صفحه‌بندی.</param>
    /// <returns>لیست محصولات به همراه اطلاعات صفحه‌بندی.</returns>
    /// 

    
        public Task<ListDataResult<ProductView>> SearchProducts(ProductSearchCriteria criteria);
    }

}
