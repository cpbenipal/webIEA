﻿
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
        public AccountManager(IRepositoryBase<User> repositoryBase, IHashManager hashManager)
        {
            _repositoryBase = repositoryBase;
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
        public User Login(LoginDto dt, long MemberId)
        {
            var encryptedText = _hashManager.EncryptPlainText(dt.Password);
            return _repositoryBase.FirstOrDefaultAsync(x => x.loginUserId == MemberId && x.Password == encryptedText);                    
        }
        public object UpdatePassword(UpdatePasswordDto dt)
        {
            AccountDto AccountDto = new AccountDto();
            dt.OldPassword = _hashManager.EncryptPlainText(dt.OldPassword);
            var model = _repositoryBase.FirstOrDefaultAsync(x => x.Id == dt.Id && x.Password == dt.OldPassword);
            if (model != null)
            {
                model.Password = dt.NewPassword;
                var result = _repositoryBase.Update(model);
                _repositoryBase.Save();
                return result;
            }
            else
            { return dt; }
        }
        public AccountDto GetById(string Id)
        {
            AccountDto AccountDto = new AccountDto();
            var model = _repositoryBase.GetById(Id);
            AccountDto.Id = model.Id;            
            AccountDto.TableName = model.TableName;
            AccountDto.RoleId = model.RoleId;
            AccountDto.loginUserId = model.loginUserId;            
            return AccountDto;
        }
        public List<AccountDto> GetAll()
        {
            var model = _repositoryBase.GetAll();
            var data = model.Select(x => new AccountDto
            {
                Id = x.Id,
                TableName = x.TableName,
                RoleId = x.RoleId,
                Password = x.Password,
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
