using MyFrameWork.AppTool;


namespace App.Contracts.Object.Shop.ProductCon
{
    public interface IProductApp
    {
        public Task<List<ProductView>> SearchProducts(ProductSearchCriteria criteria);
        Task<OPTResult<ProductView>> GetAll(Pagination pagination);
        Task<OPT> Create(ProductCreate productCreate);
        OPT DeleteBy(List<int> productids);
        //public void Dispose();
    }

}
