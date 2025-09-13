using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Objects.Shop
{
    public class Product : BaseDomain
    {
        public string? ProductCode { get; set; }
        public string Name { get; set; } = default!;
        public int Price { get; set; }
        public int CountTypeId { get; set; }
        public CountType CountType { get; set; }
    }
}
