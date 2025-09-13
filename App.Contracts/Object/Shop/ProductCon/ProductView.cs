using MyFrameWork.AppTool;
using System.ComponentModel.DataAnnotations;




namespace App.Contracts.Object.Shop.ProductCon
{
    public class ProductView
    {
       
        public int Id { get; set; }
        [Display(Name = " کد محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [MaxLength(20,ErrorMessage = MessageApp.MaxLengthcustom)]
        public string ProductCode { get; set; }

        [Display(Name = " نام محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [MaxLength(20, ErrorMessage = MessageApp.MaxLengthcustom)]
        public string Name { get; set; }

        [Display(Name = " قیمت")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]    
        
        public int Price { get; set; }
        

    }
    public class ProductCreate 
    {

        [Display(Name = " کد محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [MaxLength(20,ErrorMessage = MessageApp.MaxLengthcustom)]
        public string ProductCode { get; set; }

        [Display(Name = " نام محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [MaxLength(20, ErrorMessage = MessageApp.MaxLengthcustom)]
        public string Name { get; set; }

        [Display(Name = " قیمت")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]    
        
        public int Price { get; set; }

        [Display(Name = " واحد شمارش")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]  
        [SelectSource("/api/counttype/CountTypelist")]
        public int CountTypeId { get; set; }
       
       

    }

       public class ProductUpdate 
    {

        public int Id { get; set; }

        [Display(Name = " کد محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [MaxLength(20,ErrorMessage = MessageApp.MaxLengthcustom)]
        public string ProductCode { get; set; }

        [Display(Name = " نام محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [MaxLength(20, ErrorMessage = MessageApp.MaxLengthcustom)]
        public string Name { get; set; }

        [Display(Name = " قیمت")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]    
        
        public int Price { get; set; }

        [Display(Name = " واحد شمارش")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]  
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
