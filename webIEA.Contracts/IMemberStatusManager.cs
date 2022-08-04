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
    public interface IMemberStatusManager
    {
        object Add(StatusDto model);
        object Update(StatusDto model);
        StatusDto GetById(long id);
        List<StatusDto> GetAll();
        object Delete(int Id);
    }
}
