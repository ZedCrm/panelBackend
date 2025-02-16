using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Contracts.Object.Base
{
    public class PersonView
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public string Name { get; set; }

        [Required]
        [StringLength(3)]
        public string Family { get; set; }

        [Required]
        public int age { get; set; }
    }
}
