
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using webIEA.Contracts;
using webIEA.DataBaseContext;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Repositories
{
    public class MemberDocumentManager : IMemberDocumentManager
    {
        private readonly IRepositoryBase<MemberDocument> _repositoryBase; 
        private readonly Mapper mapper;

        public MemberDocumentManager(IRepositoryBase<MemberDocument> repositoryBase)
        { 
            _repositoryBase = repositoryBase;          
        }

        public object Add(MemberDocument model)
        {            
            var result = _repositoryBase.Insert(model);
            _repositoryBase.Save();
            return result;
        }
        public object Update(MemberDocument memberDocument)
        {            
            var result = _repositoryBase.Update(memberDocument);
            _repositoryBase.Save();
            return result;
        }
        public MemberDocumentDto GetById(long Id)
        {
            MemberDocumentDto MemberDocumentDto = new MemberDocumentDto();
            var model = _repositoryBase.GetById(Id);
            MemberDocumentDto.Id = model.ID;
            MemberDocumentDto.MemberId = model.MemberID;
            MemberDocumentDto.DocumentName = model.DocumentName;
            MemberDocumentDto.ContentType = model.ContentType;
            MemberDocumentDto.Path = model.Path;
            return MemberDocumentDto;
        }
        public MemberDocument GetFirstById(long Id) 
        {
            return _repositoryBase.GetById(Id);
        }
        public List<MemberDocumentDto> GetAll()
        {
            var model = _repositoryBase.GetAll();
            var data = model.Select(x => new MemberDocumentDto
            {
                Id = x.ID,
                MemberId = x.MemberID,
                DocumentName = x.DocumentName,
                ContentType = x.ContentType,
                Path = x.Path,
            }).ToList();
            return data;
        }
        public List<MemberDocumentDto> GetAllFiltered(long Id)
        {
            var model = _repositoryBase.GetAllFiltered(x => x.MemberID == Id);
            var data = model.Select(x => new MemberDocumentDto
            {
                Id = x.ID,
                MemberId = x.MemberID,
                DocumentName = x.DocumentName,
                ContentType = x.ContentType,
                Path = x.Path,
            }).ToList();
            return data;
        }
        public object Delete(int Id)
        {
            
            _repositoryBase.Delete(Id);
            _repositoryBase.Save();
            return "";
        }
        public object DeleteList(long Id)
        {
            _repositoryBase.DeleteList(x=>x.MemberID==Id);
            _repositoryBase.Save();
            return "";
        }
    }
}
