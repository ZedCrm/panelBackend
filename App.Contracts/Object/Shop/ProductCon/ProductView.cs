using MyFrameWork.AppTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using App.Contracts.Object.Shop.CountTypeCon;


namespace App.Contracts.Object.Shop.ProductCon
{
    public class ProductView
    {
        public int Id { get; set; }
        [Display(Name = " محصول")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]
        [MaxLength(20,ErrorMessage = " . تعداد کارکترهای {0} نمیتواند بیشتر از {1} باشد")]
        public string ProductCode { get; set; }

        [Display(Name = " نام محصول")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]
        [MaxLength(20, ErrorMessage = " . تعداد کارکترهای {0} نمیتواند بیشتر از {1} باشد")]
        public string Name { get; set; }

        [Display(Name = " قیمت")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]    
        
        public int Price { get; set; }
        

    }
    public class ProductCreate 
    {

        [Display(Name = " محصول")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]
        [MaxLength(20,ErrorMessage = " . تعداد کارکترهای {0} نمیتواند بیشتر از {1} باشد")]
        public string ProductCode { get; set; }

        [Display(Name = " نام محصول")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]
        [MaxLength(20, ErrorMessage = " . تعداد کارکترهای {0} نمیتواند بیشتر از {1} باشد")]
        public string Name { get; set; }

        [Display(Name = " قیمت")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]    
        
        public int Price { get; set; }

        [Display(Name = " واحد شمارش")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]  
        [SelectSource("/api/counttype/CountTypelist")]
        public int CountTypeId { get; set; }
       
       

    }

       public class ProductUpdate 
    {

        public int Id { get; set; }

        [Display(Name = " محصول")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]
        [MaxLength(20,ErrorMessage = " . تعداد کارکترهای {0} نمیتواند بیشتر از {1} باشد")]
        public string ProductCode { get; set; }

        [Display(Name = " نام محصول")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]
        [MaxLength(20, ErrorMessage = " . تعداد کارکترهای {0} نمیتواند بیشتر از {1} باشد")]
        public string Name { get; set; }

        [Display(Name = " قیمت")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]    
        
        public int Price { get; set; }

        [Display(Name = " واحد شمارش")]
        [Required(ErrorMessage = ". لطفا {0} را وارد کنید")]  
        [SelectSource("/api/counttype/CountTypelist")]
        public int CountTypeId { get; set; }
       
       

    }
    public class ProductSearchCriteria : Pagination
    {
        public string? Name { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
    }

}
