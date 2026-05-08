using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Objects.Shop
{
    [Table("InvTranDetails")]
    public class InvTranDetail : BaseDomain
    {
        [Required]
        public int InvTranId { get; set; }

        [ForeignKey(nameof(InvTranId))]
        public InvTran InvTran { get; set; }
    }
}