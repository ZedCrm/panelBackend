using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Objects.Shop
{
    public class CountType : BaseDomain
    {
       // [Column(TypeName = "smallint")]
        public  int Id { get; set; }
        public string Name { get; set; } = "";

        public ICollection<Product> products { get; set; } 
    }
}
