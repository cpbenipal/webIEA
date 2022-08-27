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
        [Required]
        public string TrainingName { get; set; }
        //[Required]
        public string Description { get; set; }
        [Required]
        public int ValidatedHours { get; set; }
        public bool IsShow { get; set; }
        [Required]
        public int TypeID { get; set; }
        [Required]
        public List<int> LanguageId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        [Required]
        public DateTime StartDateTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        [Required]
        public DateTime EndDateTime { get; set; }
        [Required]
        public float Cost { get; set; }
        [Required]
        public bool IsFullTime { get; set; }
        public string Location { get; set; }
        public bool IsApproved { get; set; }
        public int StatusID { get; set; }
        public List<ListCollectionDto> Languages { get; set; }
        public List<ListCollectionDto> CourseType { get; set; }

    }


}
