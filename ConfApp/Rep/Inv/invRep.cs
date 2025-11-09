using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Object.Shop.InvApp;
using Domain.Objects.Shop;

namespace ConfApp.Rep.Inv
{
    public class InvRep : BaseRep<Domain.Objects.Shop.Inv , int> , IInvRep
    {
        
        public InvRep(MyContext context): base(context)
        {
            
        }
    }
}