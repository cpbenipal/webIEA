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
    public interface IEmploymentStatusManager
    {
        object Add(EmploymentStatusDto employmentStatusDto);
        object Update(EmploymentStatusDto employmentStatusDto);
        EmploymentStatusDto GetById(long id);
        List<EmploymentStatusDto> GetAll();
        object Delete(int Id);
    }
}
