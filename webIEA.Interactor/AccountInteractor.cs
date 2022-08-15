//using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return repositoryWrapper.AccountManager.Login(model);
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
