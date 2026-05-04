using App.Contracts.Object.Base;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Object.Base
{
    public class PersonApp : IPersonApp
    {
        private readonly IPersonRep _ctx;
        public PersonApp(IPersonRep personRep)
        {
            _ctx = personRep;
        }
        public async Task<ListDataResult<PersonView>> personViews()
        {
            var persons = await _ctx.GetAsync();

            var data =  persons.Select(c => new PersonView
            {
                Id = c.Id,
                Name = c.Name,
                Family = c.Family,
                age = c.Age,

            }).ToList();


           return ResultFactory.List<PersonView>(ResultStatusEnum.Success , data , 0 ,new Pagination{});

        }

        public Task<ListDataResult<PersonView>> PersonViews()
        {
            throw new NotImplementedException();
        }
    }
}
