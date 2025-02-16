using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Objects.Shop
{
    public class InvTran : BaseDomain
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime AcceptTime{ get; set; }
        public Inv Inv { get; set; }

    }
}
