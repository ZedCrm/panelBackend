// App/Object/Base/PersonBusinessService.cs
using App.Contracts.Object.Base;
using ConfApp;
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Base
{
    public class PersonBusinessService : BaseService<Person>
    {
        public PersonBusinessService(MyContext context) : base(context)
        {
        }

        public async Task<StatusResult> ValidateUniquePersonAsync(string name, string family, int? excludeId = null)
        {
            var query = GetActiveQuery();
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            var exists = await query.AnyAsync(p => p.Name == name && p.Family == family);
            if (exists)
                return ResultFactory.Status(ResultStatusEnum.Conflict,
                    MessageApp.DuplicateField($"نام و نام خانوادگی '{name} {family}'"));

            return ResultFactory.Status(ResultStatusEnum.Success);
        }
    }
}