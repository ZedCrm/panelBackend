using MyFrameWork.AppTool.ResultType;

namespace App.Contracts.Object.Base
{
    public interface IPersonApp : ICrudService<PersonView,PersonCreate ,PersonUpdate , int>
    {
       
    }
}
