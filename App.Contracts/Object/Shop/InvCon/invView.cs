using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Contracts.Object.Shop.InvCon
{
    public class invView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Boolean Active  { get; set; }=true;
    }


    public class invCreate
    {
        
        public string Name { get; set; }
        public Boolean Active  { get; set; }=true;
    }

        public class invUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Boolean Active  { get; set; }=true;
    }

}