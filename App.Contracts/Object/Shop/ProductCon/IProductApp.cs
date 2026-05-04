using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;


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

    
        public Task<ListDataResult<ProductView>> SearchProducts(ProductSearchCriteria criteria);

        public Task<ListDataResult<ProductView>> GetAll(Pagination pagination);
        public Task<SingleDataResult<ProductUpdate>> GetById(int id);
        public Task<StatusResult> Create(ProductCreate dto);
        public Task<StatusResult> DeleteBy(List<int> ids);
        public Task<StatusResult> Update(ProductUpdate dto);
    }

}
