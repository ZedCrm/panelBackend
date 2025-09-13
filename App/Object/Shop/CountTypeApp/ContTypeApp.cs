// فایل: App/Object/Shop/CountTypeApp/CountTypeApp.cs
using App.Contracts.Object.Shop.CountTypeCon;
using App.utility;
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
                Message = MessageApp.AcceptOpt,
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
                return OPTResult<CountTypeView>.Failed(MessageApp.NotFound);

            var viewModel = _mapper.Map<CountTypeView>(entity);
            return OPTResult<CountTypeView>.Success(viewModel,MessageApp.AcceptOpt);
        }




        // متد ایجاد واحد شمارش جدید
        public async Task<OPTResult<CountTypeView>> Create(CountTypeCreate countTypeCreate)
        {
            var entity = _mapper.Map<CountType>(countTypeCreate);
            await _rep.CreateAsync(entity);
            await _rep.SaveChangesAsync();

            var viewModel = _mapper.Map<CountTypeView>(entity);
            return OPTResult<CountTypeView>.Success(viewModel, MessageApp.AcceptOpt);
        }





        // متد حذف واحد شمارش بر اساس لیست شناسه‌ها

        public async Task<OPTResult<CountTypeView>> DeleteBy(List<int> ids)
        {
            var entities = await _rep.GetByIdsAsync(ids);
            if (entities == null || entities.Count == 0)
                return OPTResult<CountTypeView>.Failed(MessageApp.NotFound);

            var deletableEntities = new List<CountType>();
            var usedEntities = new List<CountType>();

            foreach (var entity in entities)
            {
                var isUsed = await _rep.HasRelationsAsync(entity); // ✅ بررسی استفاده در جداول رابطه‌ای
                if (!isUsed)
                    deletableEntities.Add(entity);
                else
                    usedEntities.Add(entity);
            }

            if (deletableEntities.Any())
            {
                _rep.DeleteRange(deletableEntities);
                await _rep.SaveChangesAsync();
            }

            string message = "";
            if (deletableEntities.Count > 0)
                message += $"{deletableEntities.Count} واحد شمارش با موفقیت حذف شد. ";
            if (usedEntities.Count > 0)
                message += $"{usedEntities.Count} مورد به دلیل استفاده در بخش‌های دیگر حذف نشد.";

            return OPTResult<CountTypeView>.Success(message.Trim());
        }




        // متد به‌روزرسانی واحد شمارش (در صورت نیاز)
        public async Task<OPTResult<CountTypeView>> Update(CountTypeView countTypeView)
        {
            var validateAllProperties = ModelValidator.ValidateToOptResult<CountTypeView>(countTypeView);
            if (!validateAllProperties.IsSucceeded) return validateAllProperties;


            var entity = await _rep.GetAsync(countTypeView.Id);
            if (entity == null)
                return OPTResult<CountTypeView>.Failed(MessageApp.NotFound);
            _mapper.Map(countTypeView, entity);
            await _rep.UpdateAsync(entity);
            await _rep.SaveChangesAsync();
            var viewModel = _mapper.Map<CountTypeView>(entity);
            return OPTResult<CountTypeView>.Success(viewModel, MessageApp.AcceptOpt);
        }
    }

    // اینترفیس ریپوزیتوری واحد شمارش که از ریپوزیتوری پایه ارث‌بری می‌کند
    public interface ICountTypeRep : IBaseRep<CountType, int> { }



}