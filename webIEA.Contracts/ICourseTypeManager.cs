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
    public interface ICourseTypeManager
    {
        object Add(CourseTypeDto courseTypeDto);
        object Update(CourseTypeDto courseTypeDto);
        CourseTypeDto GetById(long id);
        List<CourseTypeDto> GetAll();
        List<CourseTypeDto> GetAllFiltered(long Id);
        object Delete(int Id);
    }
}
