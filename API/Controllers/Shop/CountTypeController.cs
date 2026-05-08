using App.Contracts.Object.Shop.CountTypeCon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.Shop
{

    [Authorize]
    public class CountTypeController : GenericController<CountTypeView, CountTypeCreate, CountTypeView, int>
    {
        public CountTypeController(ICountTypeApp app) : base(app)
        {
        }
    }
}