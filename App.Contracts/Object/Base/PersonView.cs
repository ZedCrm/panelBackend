// App.Contracts/Object/Base/PersonView.cs
using System.ComponentModel.DataAnnotations;
using App.Contracts.Attributes;

namespace App.Contracts.Object.Base
{
    public class PersonCreate
    {
        [Required(ErrorMessage = "لطفاً نام را وارد کنید.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "نام باید بین 2 تا 50 کاراکتر باشد.")]
        [PersianText]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "لطفاً نام خانوادگی را وارد کنید.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "نام خانوادگی باید بین 2 تا 50 کاراکتر باشد.")]
        [PersianText]
        public string Family { get; set; } = string.Empty;

        [Required(ErrorMessage = "لطفاً سن را وارد کنید.")]
        [RangeIf(1, 150)]
        [PositiveNumber]
        public int Age { get; set; }
    }

    public class PersonView : PersonCreate
    {
        public int Id { get; set; }
    }

    public class PersonUpdate : PersonView
    {
    }
}