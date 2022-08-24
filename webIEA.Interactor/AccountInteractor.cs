using System.Collections.Generic;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Interactor
{
    public class AccountInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public AccountInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
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
                loginDetail = userLogin == null ? null :  new AccountDto()
                {
                    Id = userLogin.Id,
                    Email = emailcheck.Email,
                    FirstName = emailcheck.FirstName,
                    RoleId = userLogin.RoleId,
                    loginUserId = userLogin.loginUserId
                };
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
    }
}
