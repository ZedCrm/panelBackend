using MyFrameWork.AppTool;


namespace App.Contracts.Object.Shop.CountTypeCon
{
    public interface ICountTypeApp
    {
        public Task<ApiResult<List<CountTypeView>>> GetAll(Pagination pagination);
        public Task<ApiResult<CountTypeView>> GetById(int id);
        public Task<ApiResult> Create(CountTypeCreate dto);
        public Task<ApiResult> DeleteBy(List<int> ids);
        public Task<ApiResult> Update(CountTypeView dto);
    

    }

}
