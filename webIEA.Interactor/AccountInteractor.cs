using System;
using System.Collections.Generic;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Interactor
{
    public class AccountInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryBase<UserLog> userlogrep;

        public AccountInteractor(IRepositoryWrapper _repositoryWrapper, IUnitOfWork unitOfWork)
        {
            repositoryWrapper = _repositoryWrapper;
            _unitOfWork = unitOfWork;
            userlogrep = _unitOfWork.GetRepository<UserLog>();
        }

        //public object register(AccountDto model)
        //{
        //    return repositoryWrapper.AccountManager.register(model);
        //}
        public AccountDto Login(LoginDto model)
        {
            AccountDto loginDetail = null;
            var emailcheck = repositoryWrapper.MemberManager.GetMemberByEmail(model.Email);
            if (emailcheck != null)
            {
                var userLogin = repositoryWrapper.AccountManager.Login(model, emailcheck.Id);
                if(userLogin != null)                
                {
                    loginDetail = new AccountDto()
                    {
                        Id = userLogin.Id,
                        Email = emailcheck.Email,
                        FirstName = emailcheck.FirstName,
                        RoleId = userLogin.RoleId,
                        loginUserId = userLogin.loginUserId,
                        Status = (int)emailcheck.StatusID
                    };

                    var data = new UserLog
                    {
                        UserId = userLogin.Id,
                        Login = DateTime.Now,
                    };
                    userlogrep.Insert(data);
                    userlogrep.Save();
                    loginDetail.LogId = data.Id;
                }                
            }
            return loginDetail;
        }        
        public List<AccountDto> GetAll()
        {
            return repositoryWrapper.AccountManager.GetAll();
        }
        public AccountDto GetById(string id)
        {
            return repositoryWrapper.AccountManager.GetById(id);
        }
        public object Delete(string id)
        {
            return repositoryWrapper.AccountManager.Delete(id);
        }
        public object UpdatePassword(UpdatePasswordDto model)
        {
            return repositoryWrapper.AccountManager.UpdatePassword(model);
        }
        public object Logout(long Id)
        {
            var dt = userlogrep.FirstOrDefaultAsync(x => x.Id == Id);
            if (dt != null)
            {
                dt.Logout = DateTime.Now;
                userlogrep.Update(dt);
                userlogrep.Save();
            }
            return dt;
        }
    }
}
