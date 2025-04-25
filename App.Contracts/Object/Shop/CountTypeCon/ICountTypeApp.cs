using MyFrameWork.AppTool;


namespace App.Contracts.Object.Shop.CountTypeCon
{
    public interface ICountTypeApp
    {
        public Task<OPTResult<CountTypeView>> GetAll(Pagination pagination);
        public Task<OPTResult<CountTypeView>> GetById(int id);
        public Task<OPTResult<CountTypeView>> Create(CountTypeCreate countTypeCreate);
        public Task<OPTResult<CountTypeView>> Update(CountTypeView countTypeView);
        public Task<OPTResult<CountTypeView>> DeleteBy(List<int> ids);
    }

}
