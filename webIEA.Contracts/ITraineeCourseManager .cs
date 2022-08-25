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
        int Add(TrainingCours traineeCourse);
        object Update(TrainingCours traineeCourse);
        TrainingCours GetById(long id);
        List<TrainingCours> GetAll();
        object Delete(int Id);
    }
}
