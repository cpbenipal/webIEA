
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class AccountManager : IAccountManager
    {
        private readonly IRepositoryBase<User> _repositoryBase;
        private readonly IHashManager _hashManager;
        private readonly Mapper mapper; 
        public AccountManager(IRepositoryBase<User> repositoryBase, IHashManager hashManager)
        {
            _repositoryBase = repositoryBase;
            var _mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<User, AccountDto>());
            mapper = new Mapper(_mapConfig);
            _hashManager = hashManager;
        }

        public User Register(User model)
        {
            try
            { 
                var result = _repositoryBase.Insert(model);
                _repositoryBase.Save();               
                return result;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public AccountDto Login(LoginDto dt)
        {
            AccountDto AccountDto = new AccountDto();
            dt.Password = _hashManager.EncryptPlainText(dt.Password);
            var model = _repositoryBase.FirstOrDefaultAsync(x => x.Email == dt.Email && x.Password == dt.Password);
            if (model != null)
            {
                AccountDto.Id = model.Id;
                AccountDto.Email = model.Email;
                AccountDto.TableName = model.TableName;
                AccountDto.RoleId = model.RoleId;
                AccountDto.loginUserId = model.loginUserId;
            }
            return AccountDto;
        }
        public object UpdatePassword(UpdatePasswordDto dt)
        {
            AccountDto AccountDto = new AccountDto();
            var model = _repositoryBase.FirstOrDefaultAsync(x => x.Email == dt.Email && x.Password == dt.OldPassword);
            model.Password = _hashManager.EncryptPlainText(dt.NewPassword);
            var result = _repositoryBase.Update(model);
            _repositoryBase.Save();
            return result;
        }
        public AccountDto GetById(string Id)
        {
            AccountDto AccountDto = new AccountDto();
            var model = _repositoryBase.GetById(Id);
            AccountDto.Id = model.Id;
            AccountDto.Email = model.Email;
            AccountDto.TableName = model.TableName;
            AccountDto.RoleId = model.RoleId;
            AccountDto.loginUserId = model.loginUserId;
            AccountDto.Email = model.Email;
            return AccountDto;
        }
        public List<AccountDto> GetAll()
        {
            var model = _repositoryBase.GetAll();
            var data = model.Select(x => new AccountDto
            {
                Id = x.Id,
                Email = x.Email,
                TableName = x.TableName,
                RoleId = x.RoleId,
                loginUserId = x.loginUserId,
            }).ToList();
            return data;
        }
        public List<AccountDto> GetAllFiltered(string Id)
        {
            var model = _repositoryBase.GetAllFiltered(x => x.Id == Id);
            var data = model.Select(x => new AccountDto
            {
                Id = x.Id,
                Email = x.Email,
                TableName = x.TableName,
                RoleId = x.RoleId,
                loginUserId = x.loginUserId,
            }).ToList();
            return data;
        }
        public object Delete(string Id)
        {
            _repositoryBase.Delete(Id);
            _repositoryBase.Save();
            return "";
        } 
    }
}
