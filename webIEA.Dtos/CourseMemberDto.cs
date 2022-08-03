using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Dtos
{
    public class CourseMemberDto
    {
        public long Id { get; set; }
        public long? MemberID { get; set; }
        public int? TrainingCourseId { get; set; }
       
    }


}
