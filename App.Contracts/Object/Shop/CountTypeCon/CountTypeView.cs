using MyFrameWork.AppTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace App.Contracts.Object.Shop.CountTypeCon
{
    public class CountTypeView
    {
        [Display(Name = " شناسه")]
        public int Id { get; set; }
    
        [Display(Name = " نام واحد شمارش")]
        public string Name { get; set; } = "";
        

    }
    public class CountTypeCreate 
    {

        
    
        [Display(Name = " نام واحد شمارش")]
        [Required(ErrorMessage = "لطفا نام واحد شمارش را وارد کنید")]
        [MaxLength(2, ErrorMessage = "نام واحد شمارش نمی تواند بیشتر از 50 کاراکتر باشد")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "فقط از حروف فارسی استفاده کنید")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = "";
       
       

    }
   

}
