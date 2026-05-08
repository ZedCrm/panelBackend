using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Objects.Shop
{
    [Table("Inventories")]
    public class Inv : BaseDomain
    {
        [Required]
        public string Name { get; set; }

        public bool Active { get; set; } = true;
    }
}