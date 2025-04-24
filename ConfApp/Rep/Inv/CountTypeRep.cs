using App.Object.Shop.CountTypeApp;
using App.Object.Shop.ProductApp;
using Domain.Objects.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfApp.Rep.Inv
{
    public class CountTypeRep : BaseRep<CountType, int>, ICountTypeRep
    {
        public CountTypeRep(MyContext context) : base(context) 
        {
            
        }
    }
}
