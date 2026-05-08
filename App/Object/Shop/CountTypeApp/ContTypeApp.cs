// App/Object/Shop/CountTypeApp/ContTypeApp.cs
using App.Contracts.Object.Shop.CountTypeCon;
using App.utility;
using AutoMapper;
using Domain.Objects.Shop;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using ConfApp;

namespace App.Object.Shop.CountTypeApp
{
    public class CountTypeApp : CrudService<CountType, CountTypeView, CountTypeCreate, CountTypeView, int>, ICountTypeApp
    {
        private readonly CountTypeBusinessService _countTypeBusiness;

        public CountTypeApp(MyContext context, IMapper mapper, CountTypeBusinessService countTypeBusiness)
            : base(context, mapper)
        {
            _countTypeBusiness = countTypeBusiness;
        }

        public override async Task<StatusResult> CreateAsync(CountTypeCreate dto)
        {
            var uniqueCheck = await _countTypeBusiness.ValidateUniqueNameAsync(dto.Name);
            if (!uniqueCheck.IsSuccess) return uniqueCheck;

            return await base.CreateAsync(dto);
        }

        public override async Task<StatusResult> UpdateAsync(CountTypeView dto)
        {
            var uniqueCheck = await _countTypeBusiness.ValidateUniqueNameAsync(dto.Name, dto.Id);
            if (!uniqueCheck.IsSuccess) return uniqueCheck;

            return await base.UpdateAsync(dto);
        }
    }
}