using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Shop.InvCon
{
    public class InvView
    {
        public int Id { get; set; }
        [Display(Name = "نام")]
        [MaxLength(20,ErrorMessage = MessageApp.MaxLengthcustom)]
        public string Name { get; set; }
        [Display(Name = "فعال")]
        public Boolean Active  { get; set; }=true;
    }


    public class InvCreate
    {
        
        [Display(Name = "نام")]
        [MaxLength(20,ErrorMessage = MessageApp.MaxLengthcustom)]
        public string Name { get; set; }
        [Display(Name = "فعال")]
        public Boolean Active  { get; set; }=true;
    }
    

        public class InvUpdate
    {
        public int Id { get; set; }
        [Display(Name = "نام")]
        [MaxLength(20,ErrorMessage = MessageApp.MaxLengthcustom)]
        public string Name { get; set; }
        [Display(Name = "فعال")]
        public Boolean Active  { get; set; }=true;
    
    }

}