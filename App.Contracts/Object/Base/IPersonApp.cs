using MyFrameWork.AppTool.ResultType;

namespace App.Contracts.Object.Base
{
    public interface IPersonApp
    {
        Task<ListDataResult<PersonView>> PersonViews();
    }
}
