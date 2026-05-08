using App.Contracts.Object.Shop.CountTypeCon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Shop
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CountTypeController : GenericController<CountTypeView, CountTypeCreate, CountTypeView, int>
    {
        public CountTypeController(ICountTypeApp app) : base(app)
        {
        }
    }
}