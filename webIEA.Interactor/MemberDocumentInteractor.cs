//using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Interactor
{
    public class MemberDocumentInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public MemberDocumentInteractor(IRepositoryWrapper _repositoryWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
        }


        public object Add(MemberDocumentDto model)
        {
            return repositoryWrapper.MemberDocumentManager.Add(model);
        }
        public object Update(MemberDocumentDto model)
        {
            return repositoryWrapper.MemberDocumentManager.Update(model);
        }
        public List<MemberDocumentDto> GetAll()
        {
            return repositoryWrapper.MemberDocumentManager.GetAll();
        }
        public List<MemberDocumentDto> GetAllFiltered(long Id)
        {
            return repositoryWrapper.MemberDocumentManager.GetAllFiltered(Id);
        }
     
        public MemberDocumentDto GetById(int id)
        {
            return repositoryWrapper.MemberDocumentManager.GetById(id);
        }
        public object Delete(int id)
        {
            return repositoryWrapper.MemberDocumentManager.Delete(id);
        }
      
    }
}
