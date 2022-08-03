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
    public interface IMemberSpecialization
    {
        //object Add(MemberSpecializationDto memberSpecializationDto);
        //object Update(MemberSpecializationDto memberSpecializationDto);
        //MemberSpecializationDto GetById(long id);
        //List<MemberSpecializationDto> GetAll();
        object Delete(int Id);
    }
}
