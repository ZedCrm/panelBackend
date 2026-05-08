using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Objects.Base
{
    [Table("Persons")]
    public class Person : BaseDomain
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        [MaxLength(250)]
        public string Family { get; set; }

        public int Age { get; set; }
    }
}