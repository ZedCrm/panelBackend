using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Objects.Shop
{
    [Table("InvTrans")]
    public class InvTran : BaseDomain
    {
        public DateTime CreateTime { get; set; }

        public DateTime AcceptTime { get; set; }

        [Required]
        public int InvId { get; set; }

        [ForeignKey(nameof(InvId))]
        public Inv Inv { get; set; }
    }
}