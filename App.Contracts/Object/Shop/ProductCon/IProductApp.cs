using MyFrameWork.AppTool;


namespace App.Contracts.Object.Shop.ProductCon
{
    public interface IProductApp
    {
        public Task<OPTResult<ProductView>> SearchProducts(ProductSearchCriteria criteria);
        Task<OPTResult<ProductView>> GetAll(Pagination pagination);
        Task<OPT> Create(ProductCreate productCreate);
        Task<OPT> DeleteBy(List<int> productids);
        Task<OPTResult<ProductUpdate>> GetById(int id);
        Task<OPTResult<ProductView>> Update(ProductView productView);
        
        //public void Dispose();
    }

}
