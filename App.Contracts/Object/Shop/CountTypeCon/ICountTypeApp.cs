using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;


namespace App.Contracts.Object.Shop.CountTypeCon
{
    public interface ICountTypeApp
    {
        public Task<ListDataResult<CountTypeView>> GetAll(Pagination pagination);
        public Task<SingleDataResult<CountTypeView>> GetById(int id);
        public Task<StatusResult> Create(CountTypeCreate dto);
        public Task<StatusResult> DeleteBy(List<int> ids);
        public Task<StatusResult> Update(CountTypeView dto);
    

    }

}
