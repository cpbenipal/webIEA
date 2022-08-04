using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Dtos
{
    public class MemberSpecializationDto
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public string SpecializationName { get; set; }
    }
}
