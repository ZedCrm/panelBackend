using MyFrameWork.AppTool;


namespace App.Contracts.Object.Shop.ProductCon
{
        public interface IProductApp
    {
        /// <summary>
    /// جستجوی محصولات بر اساس معیارهای مشخص (نام، حداقل و حداکثر قیمت).
    /// </summary>
    /// <param name="criteria">معیارهای جستجو شامل نام، قیمت و اطلاعات صفحه‌بندی.</param>
    /// <returns>لیست محصولات به همراه اطلاعات صفحه‌بندی.</returns>
    /// 

    
        public Task<ApiResult<List<ProductView>>> SearchProducts(ProductSearchCriteria criteria);

        public Task<ApiResult<List<ProductView>>> GetAll(Pagination pagination);
        public Task<ApiResult<ProductUpdate>> GetById(int id);
        public Task<ApiResult> Create(ProductCreate dto);
        public Task<ApiResult> DeleteBy(List<int> ids);
        public Task<ApiResult> Update(ProductUpdate dto);
    }

}
