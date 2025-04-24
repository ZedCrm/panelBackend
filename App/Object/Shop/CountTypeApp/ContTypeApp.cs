// فایل: App/Object/Shop/CountTypeApp/CountTypeApp.cs
using App.Contracts.Object.Shop.CountTypeCon;
using AutoMapper;
using Domain.Objects.Shop;
using MyFrameWork.AppTool;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Object.Shop.CountTypeApp
{
    public class CountTypeApp : ICountTypeApp
    {
        private readonly ICountTypeRep _rep;
        private readonly IMapper _mapper;

        public CountTypeApp(ICountTypeRep rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }

        // متد دریافت همه واحدهای شمارش با صفحه‌بندی
        public async Task<OPTResult<CountTypeView>> GetAll(Pagination pagination)
        {
            var entities = await _rep.GetAsync(pagination);
            var viewModels = _mapper.Map<List<CountTypeView>>(entities);
            var totalRecords = await _rep.CountAsync();
        
            return new OPTResult<CountTypeView>
            {
                IsSucceeded = true,
                Message = "دریافت لیست با موفقیت انجام شد.",
                Data = viewModels,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalRecords = totalRecords,
                TotalPages = pagination.CalculateTotalPages(totalRecords)
            };
        }

        // متد دریافت یک واحد شمارش بر اساس شناسه
        public async Task<OPTResult<CountTypeView>> GetById(int id)
        {
            var entity = await _rep.GetAsync(id);
            if (entity == null)
                return OPTResult<CountTypeView>.Failed("واحد شمارش یافت نشد.");

            var viewModel = _mapper.Map<CountTypeView>(entity);
            return OPTResult<CountTypeView>.Success(viewModel, "واحد شمارش با موفقیت دریافت شد.");
        }

        // متد ایجاد واحد شمارش جدید
        public async Task<OPTResult<CountTypeView>> Create(CountTypeCreate countTypeCreate)
        {
            var entity = _mapper.Map<CountType>(countTypeCreate);
            await _rep.CreateAsync(entity);
            await _rep.SaveChangesAsync();

            var viewModel = _mapper.Map<CountTypeView>(entity);
            return OPTResult<CountTypeView>.Success(viewModel, "واحد شمارش با موفقیت ایجاد شد.");
        }
    }

    // اینترفیس ریپوزیتوری واحد شمارش که از ریپوزیتوری پایه ارث‌بری می‌کند
    public interface ICountTypeRep : IBaseRep<CountType, int> { }


 
}