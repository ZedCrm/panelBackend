using System;
using System.Collections.Generic;
using API.Attributes;
using App.Contracts.Object.Shop.InvCon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace API.Controllers.Shop
{
    [Route("[controller]")]
    [Authorize]
    public class InvController :  GenericController<InvView , InvCreate, InvUpdate,int>
    {
      
        public InvController(IInvApp invApp) : base(invApp)
        {
           
        }


    }
}