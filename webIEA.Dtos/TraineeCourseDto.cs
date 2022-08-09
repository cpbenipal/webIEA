using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Dtos
{
    public class TraineeCourseDto
    {
        public long Id { get; set; }
        public string TrainingName { get; set; }
        public string Description { get; set; }
        public int ValidatedHours { get; set; }
        public bool IsShow { get; set; }
        public int TypeID { get; set; }
        public List<int> LanguageId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public float Cost { get; set; }
        public bool IsFullTime { get; set; }
        public string Location { get; set; }
        public bool IsApproved { get; set; }
        public int StatusID { get; set; }
        public List<ListCollectionDto> Languages { get; set; }
        public List<ListCollectionDto> CourseType { get; set; }

    }


}
