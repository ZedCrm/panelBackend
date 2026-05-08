// App/Object/Base/PersonApp.cs
using App.Contracts.Object.Base;
using AutoMapper;
using ConfApp;
using Domain.Objects.Base;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Base
{
    public class PersonApp : CrudService<Person, PersonView, PersonCreate, PersonUpdate, int>, IPersonApp
    {
        private readonly PersonBusinessService _personBusiness;

        public PersonApp(MyContext ctx, IMapper mapper, PersonBusinessService personBusiness)
            : base(ctx, mapper)
        {
            _personBusiness = personBusiness;
        }

        public override async Task<StatusResult> CreateAsync(PersonCreate dto)
        {
            // Validation یکتایی نام و نام خانوادگی
            var uniqueCheck = await _personBusiness.ValidateUniquePersonAsync(dto.Name, dto.Family);
            if (!uniqueCheck.IsSuccess) return uniqueCheck;

            // Validation سن و سایر موارد به صورت خودکار توسط ModelValidator انجام می‌شود
            return await base.CreateAsync(dto);
        }

        public override async Task<StatusResult> UpdateAsync(PersonUpdate dto)
        {
            var uniqueCheck = await _personBusiness.ValidateUniquePersonAsync(dto.Name, dto.Family, dto.Id);
            if (!uniqueCheck.IsSuccess) return uniqueCheck;

            return await base.UpdateAsync(dto);
        }
    }
}