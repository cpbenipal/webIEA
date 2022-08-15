using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Dtos
{
    public enum MemberStatusEnum
    {
        Suspended=1,
        Active,
        Pending,
        Declined
    }
}
