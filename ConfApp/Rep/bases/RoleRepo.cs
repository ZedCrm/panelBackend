using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Objects.Base;

namespace ConfApp.Rep.bases
{
    public class RoleRepo : BaseRep<Role, int>, IRoleRep
    {
        private readonly MyContext ctx;
        public RoleRepo(MyContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }
    }
}