// App/Object/Shop/CountTypeApp/CountTypeBusinessService.cs
using App.Contracts.Object.Shop.CountTypeCon;
using App.utility;
using AutoMapper;
using ConfApp;
using Domain.Objects.Shop;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Shop.CountTypeApp
{
    public class CountTypeBusinessService : BaseService<CountType>
    {
        public CountTypeBusinessService(MyContext context) : base(context)
        {
        }

        public async Task<StatusResult> ValidateUniqueNameAsync(string name, int? excludeId = null)
        {
            var query = GetActiveQuery();
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            var exists = await query.AnyAsync(c => c.Name == name);
            if (exists)
                return ResultFactory.Status(ResultStatusEnum.Conflict, MessageApp.DuplicateField("نام"));

            return ResultFactory.Status(ResultStatusEnum.Success);
        }
    }
}
