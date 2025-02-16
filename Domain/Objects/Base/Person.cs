using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Objects.Base
{
    public class Person :BaseDomain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public int age { get; set; }
    }
}
