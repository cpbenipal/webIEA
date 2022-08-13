
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class AccountManager : IAccountManager
    {
        private readonly IRepositoryBase<User> _repositoryBase;
        private readonly Mapper mapper;

        public AccountManager(IRepositoryBase<User> repositoryBase)
        {
            _repositoryBase = repositoryBase;
            var _mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<User, AccountDto>());
            mapper = new Mapper(_mapConfig);
        }

        public object register(AccountDto model)
        {
            var password = GenratePassword();
            var hashpass = Encrypt.GetMD5Hash(password);
            var data = new User
            {
                Email = model.Email,
                Password = hashpass,
                RoleId = model.RoleId,
                loginUserId = model.loginUserId,
                TableName = model.TableName,
            };
            var result = _repositoryBase.Insert(data);
            _repositoryBase.Save();
            Email.SendEmail(model.Email, "Account Details", $"your login email is {model.Email} and password is {password}");
            return result;
        }
        public AccountDto Login(LoginDto dt)
        {
            AccountDto AccountDto = new AccountDto();
            dt.Password = Encrypt.GetMD5Hash(dt.Password);
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
            model.Password = Encrypt.GetMD5Hash(dt.NewPassword);
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
        public string GenratePassword()
        {
            string allowedChars = "";

            allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";

            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";

            allowedChars += "1,2,3,4,5,6,7,8,9,0,!,@,#,$,%,&,?";

            char[] sep = { ',' };

            string[] arr = allowedChars.Split(sep);

            string passwordString = "";

            string temp = "";

            Random rand = new Random();

            for (int i = 0; i < 8; i++)

            {

                temp = arr[rand.Next(0, arr.Length)];

                passwordString += temp;

            }

            return passwordString;
        }
    }
}
