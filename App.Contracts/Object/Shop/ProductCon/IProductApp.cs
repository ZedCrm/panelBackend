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

    
        public Task<OPTResult<ProductView>> SearchProducts(ProductSearchCriteria criteria);
        Task<OPTResult<ProductView>> GetAll(Pagination pagination, int userId);
        Task<OPT> Create(ProductCreate productCreate);
        Task<OPT> DeleteBy(List<int> productids);
        Task<OPTResult<ProductUpdate>> GetById(int id);
        Task<OPTResult<ProductView>> Update(ProductView productView);
        
        //public void Dispose();
    }

}
