using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Objects.Shop
{
    [Table("Products")]
    public class Product : BaseDomain
    {
        public string? ProductCode { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = default!;

        [Required]
        public int Price { get; set; }

        [Required]
        public int CountTypeId { get; set; }

        [ForeignKey(nameof(CountTypeId))]
        public CountType CountType { get; set; }
    }
}