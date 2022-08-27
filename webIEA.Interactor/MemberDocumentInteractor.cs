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
            model.MemberId = id;
            var fileName = Guid.NewGuid().ToString() + Path.GetFileName(file.FileName);
            model.ContentType = Path.GetExtension(file.FileName);            
            model.DocumentName = Path.GetFileName(file.FileName);
            model.Path = fileName;
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images"), fileName);
            file.SaveAs(path);

            var data = new MemberDocument
            {
                MemberID = model.MemberId,
                DocumentName = model.DocumentName,
                ContentType = model.ContentType,
                Path = model.Path,
                AddedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };

            return repositoryWrapper.MemberDocumentManager.Add(data);
        }
        public object Update(MemberDocumentDto MemberDocumentDto) 
        {
            var data = repositoryWrapper.MemberDocumentManager.GetFirstById(MemberDocumentDto.Id);
            data.DocumentName = MemberDocumentDto.DocumentName;
            data.MemberID = MemberDocumentDto.MemberId;
            data.Path = MemberDocumentDto.Path;
            data.ContentType = MemberDocumentDto.ContentType;
            data.ModifiedOn = DateTime.Now;
            data.ModifiedBy = MemberDocumentDto.MemberId ?? 0;
            return repositoryWrapper.MemberDocumentManager.Update(data);
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
            MemberDocumentDto MemberDocumentDto = new MemberDocumentDto();
            var model = repositoryWrapper.MemberDocumentManager.GetFirstById(id);
            MemberDocumentDto.Id = model.ID;
            MemberDocumentDto.MemberId = model.MemberID;
            MemberDocumentDto.DocumentName = model.DocumentName;
            MemberDocumentDto.ContentType = model.ContentType;
            MemberDocumentDto.Path = model.Path;
            return MemberDocumentDto;
        }
        public object Delete(int id)
        {
            return repositoryWrapper.MemberDocumentManager.Delete(id);
        }
      
    }
}
