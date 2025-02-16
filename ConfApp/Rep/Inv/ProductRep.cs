using App.Object.Shop.ProductApp;
using Domain.Objects.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfApp.Rep.Inv
{
    public class ProductRep : BaseRep<Product, int>, IProductRep
    {
        public ProductRep(MyContext context) : base(context) 
        {
            
        }
    }
}
