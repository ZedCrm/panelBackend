using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.Users;
using App.Object.Base.Auth;
using AutoMapper;
using Domain.Objects.Base;
using MyFrameWork.AppTool;

namespace App.Object.Base.Users
{
    public class UsersApp : IUsersApp
    {


        #region constructor
        private readonly IMyUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPermissionService _PermissionService;

        public UsersApp(IMyUserRepository userRepository, IMapper mapper , IPermissionService permissionService)
        {
            _userRepository = userRepository;
            this._mapper = mapper;
            _PermissionService = permissionService ;
        }
        #endregion

        public Task<OPT> Create(UsersCreat objectCreate)
        {
            throw new NotImplementedException();
        }

        public Task<OPT> DeleteBy(List<int> objectids)
        {
            throw new NotImplementedException();
        }

        public async Task<OPTResult<UsersView>> GetAll(Pagination pagination, int objectId)
        {

            
            bool hasEditPermission = await _PermissionService.HasPermissionAsync(objectId, "ViewProduct");
            if(!hasEditPermission) if (!hasEditPermission)
    {
        return new OPTResult<UsersView>
        {
            IsSucceeded = false,
            Message = MessageApp.NotPermission,
         
        };
    } ;

            var data =  await _userRepository.GetAsync();

            var users = _mapper.Map<List<UsersView>>(data);

            

            // تعداد کل رکوردها  
            var totalRecords = await _userRepository.CountAsync();



            // تعداد کل صفحات  
            var totalPages = pagination.CalculateTotalPages(totalRecords);

            // آماده‌سازی و بازگشت نتیجه  
            return new OPTResult<UsersView>
            {
                IsSucceeded = true,
                Message = MessageApp.AcceptOpt,
                Data = users,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };


           
        }

        public Task<OPTResult<UsersCreat>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OPTResult<UsersView>> Update(UsersView objectView)
        {
            throw new NotImplementedException();
        }
    }


    public interface IMyUserRepository : IBaseRep<User , int>{
        
    }
}

