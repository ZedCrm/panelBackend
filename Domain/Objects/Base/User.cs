using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Objects.Base
{
    public class User : BaseDomain
    {

        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;

    }
}