using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Shop.InvCon
{
    public class invView
    {
        public int Id { get; set; }
        [Display(Name = "نام")]
        [MaxLength(20,ErrorMessage = MessageApp.MaxLengthcustom)]
        public string Name { get; set; }
        [Display(Name = "فعال")]
        public Boolean Active  { get; set; }=true;
    }


    public class invCreate
    {
        
        [Display(Name = "نام")]
        [MaxLength(20,ErrorMessage = MessageApp.MaxLengthcustom)]
        public string Name { get; set; }
        [Display(Name = "فعال")]
        public Boolean Active  { get; set; }=true;
    }
    

        public class invUpdate
    {
        public int Id { get; set; }
        [Display(Name = "نام")]
        [MaxLength(20,ErrorMessage = MessageApp.MaxLengthcustom)]
        public string Name { get; set; }
        [Display(Name = "فعال")]
        public Boolean Active  { get; set; }=true;
    
    }

}