//using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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


        public object UploadDocument(long id, HttpPostedFileBase file)
        {
            MemberDocumentDto model = new MemberDocumentDto();
            model.Id = id;
            var fileName = Guid.NewGuid().ToString() + Path.GetFileName(file.FileName);
            model.ContentType = Path.GetExtension(file.FileName);            
            model.DocumentName = Path.GetFileName(file.FileName);
            model.Path = fileName;
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images"), fileName);
            file.SaveAs(path);
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
