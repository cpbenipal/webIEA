
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

        public object Add(MemberDocumentDto model)
        {
            var data = new MemberDocument
            {
                MemberID = model.MemberId,
                DocumentName = model.DocumentName,
                ContentType = model.ContentType,
                Path=model.Path,
                AddedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };
            var result = _repositoryBase.Insert(data);
            _repositoryBase.Save();
            return result;
        }
        public object Update(MemberDocumentDto MemberDocumentDto)
        {
            var data = _repositoryBase.GetById(MemberDocumentDto.Id);
            data.DocumentName = MemberDocumentDto.DocumentName;
            data.MemberID = MemberDocumentDto.MemberId;
            data.Path = MemberDocumentDto.Path;
            data.ContentType = MemberDocumentDto.ContentType;
            var result = _repositoryBase.Update(data);
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
