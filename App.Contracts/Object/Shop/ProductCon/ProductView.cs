// App.Contracts/Object/Shop/ProductCon/ProductView.cs
using System.ComponentModel.DataAnnotations;
using MyFrameWork.AppTool;
using App.Contracts.Attributes;

namespace App.Contracts.Object.Shop.ProductCon
{
    public class ProductView
    {
        public int Id { get; set; }

        [Display(Name = "کد محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "کد محصول باید بین 3 تا 20 کاراکتر باشد.")]
        public string ProductCode { get; set; } = string.Empty;

        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "نام محصول باید بین 2 تا 50 کاراکتر باشد.")]
        [PersianText]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "قیمت")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [PositiveNumber]
        [RangeIf(1000, 100000000)]
        public int Price { get; set; }
    }

    public class ProductCreate
    {
        [Display(Name = "کد محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "کد محصول باید بین 3 تا 20 کاراکتر باشد.")]
        public string ProductCode { get; set; } = string.Empty;

        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "نام محصول باید بین 2 تا 50 کاراکتر باشد.")]
        [PersianText]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "قیمت")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        [PositiveNumber]
        [RangeIf(1000, 100000000)]
        public int Price { get; set; }

        [Display(Name = "واحد شمارش")]
        [Required(ErrorMessage = MessageApp.IsRequiredcustom)]
        public int CountTypeId { get; set; }
    }

    public class ProductUpdate : ProductCreate
    {
        [Required]
        public int Id { get; set; }
    }

    public class ProductSearchCriteria : Pagination
    {
        public string? Name { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
    }
}