using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.Users;
using App.Object.Base.Auth;
using App.utility;
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






        public async Task<OPT> Create(UsersCreat objectCreate)
        {
            var validateAllProperties = ModelValidator.ValidateToOpt<UsersCreat>(objectCreate);
            if (!validateAllProperties.IsSucceeded) return validateAllProperties ;


            // check uniq email
            var uniqueOpt = await ValidationUtility.ValidateUniqueAsync<User, int>(
                _userRepository,
                c => c.Email == objectCreate.Email,
                
                MessageApp.DuplicateField(objectCreate.Email)
            );
            if (!uniqueOpt.IsSucceeded) return uniqueOpt;

            // check uniq username
             uniqueOpt = await ValidationUtility.ValidateUniqueAsync<User, int>(
                _userRepository,
                c => c.Username == objectCreate.Username,
                
                MessageApp.DuplicateField(objectCreate.Username)
            );
            if (!uniqueOpt.IsSucceeded) return uniqueOpt;


            var user = _mapper.Map<User>(objectCreate);
            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();


            var opt = new OPT();
            return opt.Succeeded(MessageApp.CustomAddsuccses(objectCreate.Username));
        }








        public async Task<OPT> DeleteBy(List<int> objectids)
        {
            
            
             var opt = new OPT();
            try
            {
                if (objectids == null || !objectids.Any())
                {
                    opt.Failed(MessageApp.NotFound);
                    return opt;
                }
                foreach (var productid in objectids)
                    _userRepository.DeleteById(productid);

                await _userRepository.SaveChangesAsync();
                 opt.Succeeded(MessageApp.CustomSuccess("حذف"));
            }
            catch (Exception ex)
            {
                 opt.Failed(MessageApp.CustomDeleteFail(ex.Message));
            }

            return opt ;
        }
        




        public async Task<OPTResult<UsersView>> GetAll(Pagination pagination, int objectId)
        {


            bool hasEditPermission = await _PermissionService.HasPermissionAsync(objectId, "ViewProduct");
            if (!hasEditPermission) if (!hasEditPermission)
                {
                    return new OPTResult<UsersView>
                    {
                        IsSucceeded = false,
                        Message = MessageApp.NotPermission,

                    };
                }
            ;

            var data = await _userRepository.GetAsync();

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

        public async Task<OPTResult<UsersCreat>> GetById(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null) { return new OPTResult<UsersCreat> { IsSucceeded = false, Message =MessageApp.NotFound}; }
            var userCreat = _mapper.Map<UsersCreat>(user);
            return OPTResult<UsersCreat>.Success(userCreat, MessageApp.AcceptOpt);
        }





        public async Task<OPT> Update(UsersCreat objectView)
        {
            var validateAllProperties = ModelValidator.ValidateToOpt<UsersCreat>(objectView);
            if (!validateAllProperties.IsSucceeded) return validateAllProperties;


            // check uniq email
            var uniqueOpt = await ValidationUtility.ValidateUniqueAsync<User, int>(
                _userRepository,
                c => c.Email == objectView.Email && c.Id != objectView.Id,


                MessageApp.DuplicateField(objectView.Email)
            );
            if (!uniqueOpt.IsSucceeded) return uniqueOpt;

            // check uniq username
            uniqueOpt = await ValidationUtility.ValidateUniqueAsync<User, int>(
               _userRepository,
               c => c.Username == objectView.Username && c.Id != objectView.Id,

               MessageApp.DuplicateField(objectView.Username)
           );
            if (!uniqueOpt.IsSucceeded) return uniqueOpt;
            
                  else
            {
                var user = await _userRepository.GetAsync(objectView.Id);
                
                user.Username = objectView.Username;
                user.Email = objectView.Email;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(objectView.Password);
                
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
                return  uniqueOpt.Succeeded(MessageApp.AcceptOpt);
            }

        }


        
    }


    public interface IMyUserRepository : IBaseRep<User , int>{
        
    }
}

