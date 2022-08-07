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
    public interface ICourseMemberManager
    {
        object Add(CourseMemberDto courseMemberDto);
        object Update(CourseMemberDto courseMemberDto);
        CourseMemberDto GetById(long id);
        List<CourseMemberDto> GetAll();
        List<CourseMemberDto> GetAllFiltered(long Id);
        object Delete(int Id);
        object DeleteList(long Id);
    }
}
