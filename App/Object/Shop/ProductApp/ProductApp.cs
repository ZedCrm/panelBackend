using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Shop.ProductCon;
using App.utility;
using AutoMapper;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;
using System.Linq.Expressions;
using System.Runtime.InteropServices;


namespace App.Object.Shop.ProductApp
{
    public class ProductApp : IProductApp
    {
        #region constructor
        private readonly IProductRep _productRep;
        private readonly IMapper _mapper;
        private readonly IPermissionService _PermissionService;

        public ProductApp(IProductRep productRep, IMapper mapper , IPermissionService permissionService)
        {
            _productRep = productRep;
            this._mapper = mapper;
            _PermissionService = permissionService ;
        }
        #endregion



        //Create method 
        public async Task<OPT> Create(ProductCreate productCreate)
        {


                var validationOpt = ModelValidator.ValidateToOpt(productCreate);
                if (!validationOpt.IsSucceeded) return validationOpt;


            var opt = new OPT();


            // اعتبارسنجی تکراری بودن کد محصول
            var uniqueOpt = await ValidationUtility.ValidateUniqueAsync<Product, int>(
                _productRep,
                c => c.ProductCode == productCreate.ProductCode,
                ". کد محصول تکراری است"
            );
            if (!uniqueOpt.IsSucceeded) return uniqueOpt;

            // عملیات ایجاد محصول
            var product = _mapper.Map<Product>(productCreate);
            await _productRep.CreateAsync(product);
            await _productRep.SaveChangesAsync();

            return opt.Succeeded(MessageApp.CustomAddsuccses(productCreate.Name));
        }





        public async Task<OPT> DeleteBy(List<int> productids)
        {
            var opt = new OPT();
            try
            {
                if (productids == null || !productids.Any())
                {
                    opt.Failed(MessageApp.NotFound);
                    return opt;
                }
                foreach (var productid in productids)
                    _productRep.DeleteById(productid);

                await _productRep.SaveChangesAsync();
                 opt.Succeeded(MessageApp.CustomSuccess("حذف"));
            }
            catch (Exception ex)
            {
                 opt.Failed(MessageApp.CustomDeleteFail(ex.Message));
            }

            return opt ;
        }




        public async Task<OPTResult<ProductView>> GetAll(Pagination pagination,int userId)
        {  

            bool hasEditPermission = await _PermissionService.HasPermissionAsync(userId, "ViewProduct");
            if(!hasEditPermission) if (!hasEditPermission)
    {
        return new OPTResult<ProductView>
        {
            IsSucceeded = false,
            Message = MessageApp.NotPermission,
         
        };
    } ;
            
              // دریافت تمام محصولات  
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
                Message = MessageApp.AcceptOpt,
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
                Message = MessageApp.AcceptOpt,
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
            if (product == null) { return new OPTResult<ProductUpdate> { IsSucceeded = false, Message =MessageApp.NotFound}; }
            var productupdate = _mapper.Map<ProductUpdate>(product);
            return OPTResult<ProductUpdate>.Success(productupdate, MessageApp.AcceptOpt);
        }

        public async Task<OPTResult<ProductView>> Update(ProductView productView)
        {
            var product = await _productRep.GetAsync(productView.Id);
            if (product == null) { return new OPTResult<ProductView> { IsSucceeded = false, Message= MessageApp.AcceptOpt }; }
            var codeExist = await _productRep.ExistAsync(c => c.ProductCode == productView.ProductCode && c.Id != productView.Id);
            if (codeExist) { return new OPTResult<ProductView> { IsSucceeded = false, Message = MessageApp.DuplicateField(productView.ProductCode) }; }
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
