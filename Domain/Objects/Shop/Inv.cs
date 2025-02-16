using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Objects.Shop
{
    public class Inv : BaseDomain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Boolean Active  { get; set; }=true;

    }
}
