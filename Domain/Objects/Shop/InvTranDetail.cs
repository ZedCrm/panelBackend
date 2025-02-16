using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Objects.Shop
{
    public class InvTranDetail : BaseDomain
    {
        public int id {  get; set; }
        public InvTran InvTran { get; set; }

    }
}
