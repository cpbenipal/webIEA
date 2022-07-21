using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webIEA.Contracts;
using webIEA.Dtos;

namespace webIEA.Interactor
{
    public class MembersInteractor 
    {
        private readonly IRepositoryWrapper _repository;

        public MembersInteractor(IRepositoryWrapper repository)
        {
            _repository = repository; 
        } 

        public List<MembersDto> GetAllMembers() 
        {
            try
            {
                return null; 
            }
            catch (Exception)
            {
                // Do Logging And Prepare Error Messages Here
                throw;
            }
        }
    }
}
