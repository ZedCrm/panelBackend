using App.Contracts.Object.Shop.ProductCon;
using AutoMapper;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Object.Shop.ProductApp
{
    public class ProductApp : IProductApp
    {
        #region constructor
        private readonly IProductRep _productRep;
        private readonly IMapper _mapper;

        public ProductApp(IProductRep productRep, IMapper mapper)
        {
            _productRep = productRep;
            this._mapper = mapper;
            Console.WriteLine($"ProductApp Instance Created: {GetHashCode()}");
        }
        #endregion



        //Create method 
        public async Task<OPT> Create(ProductCreate productCreate)
        {
            OPT opt = new OPT();
            var codeExist = await _productRep.ExistAsync(c=>c.ProductCode == productCreate.ProductCode);
            if (codeExist) { opt.Failed(" . کد محصول تکراریست "); }
            else if (productCreate.Price <= 0) { opt.Failed(" . قیمت محصول باید بیشتر از صفر باشد"); }
            else
            {
                var product = _mapper.Map<Product>(productCreate);
               await  _productRep.CreateAsync(product);
                opt.Succeeded($".محصول مورد نظر ایجاد شد  {productCreate.Name.ToString()}");
                await _productRep.SaveChangesAsync();
            }

            return opt;
        }





        public OPT DeleteBy(List<int> productids)
        {
            OPT opt = new OPT();

            foreach (var productid in productids)
            {
                _productRep.DeleteById(productid);  // حذف محصول با id خاص
            }

            // صبر کردن برای اتمام عملیات حذف و ذخیره تغییرات
            _productRep.SaveChangesAsync();

            opt.Succeeded("محصولات حذف گردید");
            return opt;
        }




        public async Task<OPTResult<ProductView>> GetAll(Pagination pagination)
        {    // دریافت تمام محصولات  
            var products = await _productRep.GetAsync(pagination);

            // تبدیل داده‌ها به نوع ViewModel  
            var data =  _mapper.Map<List<ProductView>>(products);

            // تعداد کل رکوردها  
            var totalRecords = await _productRep.CountAsync();

            

            // تعداد کل صفحات  
            var totalPages = pagination.CalculateTotalPages(totalRecords);

            // آماده‌سازی و بازگشت نتیجه  
            return new OPTResult<ProductView>
            {
                IsSucceeded = true,
                Message = "داده با موفقیت بارگذاری شد.",
                Data = data,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

        }





        public async Task<List<ProductView>> SearchProducts(ProductSearchCriteria criteria )
        {
            Expression<Func<Product, bool>> filter = product => true; 

            if (!string.IsNullOrEmpty(criteria.Name))
            {
                filter = filter.And(product => product.Name.Contains(criteria.Name));   
            }

            if (criteria.MinPrice.HasValue && criteria.MinPrice > 0)
            {
                filter = filter.And(product => product.Price >= criteria.MinPrice.Value);
            }

            if (criteria.MaxPrice.HasValue && criteria.MaxPrice > 0)
            {
                filter = filter.And(product => product.Price <= criteria.MaxPrice.Value);
            }


           var products = await _productRep.GetFilteredAsync(filter , criteria);
            return _mapper.Map<List<ProductView>>(products);
        }
     
    }





    public interface IProductRep : IBaseRep<Product, int> { }
}
