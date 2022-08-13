using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Contracts
{
    public interface IAccountManager
    {
        object register(AccountDto model);
        AccountDto Login(LoginDto loginDto);
        AccountDto GetById(string id);
        List<AccountDto> GetAll();
        List<AccountDto> GetAllFiltered(string Id);
        object Delete(string Id);
        object UpdatePassword(UpdatePasswordDto dt);
    }
}
