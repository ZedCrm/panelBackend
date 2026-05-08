using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Objects.Shop
{
    [Table("CountTypes")]
    public class CountType : BaseDomain
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = default!;

        public ICollection<Product> products { get; set; }
    }
}