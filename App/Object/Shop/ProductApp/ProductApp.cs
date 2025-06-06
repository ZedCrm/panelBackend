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
            var codeExist = await _productRep.ExistAsync(c => c.ProductCode == productCreate.ProductCode);
            if (codeExist) { opt.Failed("  کد محصول تکراریست ."); }
            else if (productCreate.Price <= 0) { opt.Failed("  قیمت محصول باید بیشتر از صفر باشد"); }
            else
            {
                var product = _mapper.Map<Product>(productCreate);
                await _productRep.CreateAsync(product);
                opt.Succeeded($".محصول مورد نظر ایجاد شد  {productCreate.Name.ToString()}");
                await _productRep.SaveChangesAsync();
            }

            return opt;
        }





        public async Task<OPT> DeleteBy(List<int> productids)
        {
            OPT opt = new OPT();

            foreach (var productid in productids)
            {
                _productRep.DeleteById(productid);
            }

            await _productRep.SaveChangesAsync();

            opt.Succeeded("محصولات حذف گردید");
            return opt;
        }




        public async Task<OPTResult<ProductView>> GetAll(Pagination pagination)
        {    // دریافت تمام محصولات  
            var products = await _productRep.GetAsync(pagination);

            // تبدیل داده‌ها به نوع ViewModel  
            var data = _mapper.Map<List<ProductView>>(products);

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





        public async Task<OPTResult<ProductView>> SearchProducts(ProductSearchCriteria criteria)
        {
            // تعریف اولیه فیلتر جستجو
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

            // گرفتن داده‌ها از ریپازیتوری بر اساس فیلتر و اطلاعات صفحه‌بندی
            var products = await _productRep.GetFilteredAsync(filter, criteria);

            // نگاشت به لیست ViewModel
            var data = _mapper.Map<List<ProductView>>(products);

            // گرفتن تعداد کل رکوردها برای صفحه‌بندی
            var totalRecords = await _productRep.CountAsync(filter);
            var totalPages = criteria.CalculateTotalPages(totalRecords); 

            return new OPTResult<ProductView>
            {
                IsSucceeded = true,
                Message = "داده با موفقیت بارگذاری شد.",
                Data = data,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize
            };
        }











        public async Task<OPTResult<ProductUpdate>> GetById(int id)
        {
            var product = await _productRep.GetAsync(id);
            if (product == null) { return new OPTResult<ProductUpdate> { IsSucceeded = false, Message = "محصول یافت نشد" }; }
            var productupdate = _mapper.Map<ProductUpdate>(product);
            return OPTResult<ProductUpdate>.Success(productupdate, "محصول با موفقیت بارگذاری شد");
        }

        public async Task<OPTResult<ProductView>> Update(ProductView productView)
        {
            var product = await _productRep.GetAsync(productView.Id);
            if (product == null) { return new OPTResult<ProductView> { IsSucceeded = false, Message = "محصول یافت نشد" }; }
            var codeExist = await _productRep.ExistAsync(c => c.ProductCode == productView.ProductCode && c.Id != productView.Id);
            if (codeExist) { return new OPTResult<ProductView> { IsSucceeded = false, Message = "کد محصول تکراریست" }; }
            else
            {
                product.Name = productView.Name;
                product.Price = productView.Price;
                product.ProductCode = productView.ProductCode;
                await _productRep.UpdateAsync(product);
                await _productRep.SaveChangesAsync();
                return OPTResult<ProductView>.Success(_mapper.Map<ProductView>(product));
            }
        }


    }





    public interface IProductRep : IBaseRep<Product, int> { }
}
