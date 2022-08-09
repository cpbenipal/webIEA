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
    public interface ITraineeCourseManager
    {
        int Add(TraineeCourseDto traineeCourseDto);
        object Update(TraineeCourseDto traineeCourseDto);
        TraineeCourseDto GetById(long id);
        List<TraineeCourseDto> GetAll();
        object Delete(int Id);
    }
}
