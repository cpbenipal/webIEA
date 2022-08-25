//using AutoMapper;
using Flexpage.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using webIEA.Contracts;
using webIEA.Dtos;
using webIEA.Entities;

namespace webIEA.Interactor
{
    public class HistoryChangesInteractor
    {
        private readonly IRepositoryWrapper repositoryWrapper; protected readonly ILanguageRepository _ocessor;

        public HistoryChangesInteractor(IRepositoryWrapper _repositoryWrapper, ILanguageRepository ocessor)
        {
            repositoryWrapper = _repositoryWrapper;
            _ocessor = ocessor;

        }

        public List<HistoryDataChanx> GetAll()
        {
            return repositoryWrapper.HistoryChangesManager.GetAll();

        }

        public List<HistoryChangesListDto> GetMemberHistoryLogs(long MemberId)
        {
            var history = GetHistory(MemberId, "MemberProfile");

            return history.Select(x => new HistoryChangesListDto()
            {
                ID = x.ID,
                UpdateDate = ((DateTime)x.UpdateDate).ToShortDateString(),
                Date = x.UpdateDate.ToString(),
                PK = x.PK,
            }).ToList();
        }
        private List<HistoryDataChanx> GetHistory(long MemberId, string TableName)
        {

            var list = repositoryWrapper.HistoryChangesManager.GetAll().Where(x => x.PK == Convert.ToString(MemberId) && x.TableName == TableName && x.Type == "U").ToList();
            var groupedList = list.GroupBy(x => x.UpdateDate.Value.ToShortDateString()).Select(x => x.FirstOrDefault()).ToList();
            return groupedList;
        }
        public static string GetTypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);

            bool isNullableType = nullableType != null;

            if (isNullableType)
                return nullableType.Name;
            else
                return type.Name;
        }
        public MembersDto GetHistoryDetail(string pk, DateTime date)
        {
            var hdt = repositoryWrapper.HistoryChangesManager.GetHistoryDetail(pk, date);
            long memberId = long.Parse(pk);
            var data = repositoryWrapper.MemberManager.GetMemberById(memberId);

            foreach (var historydata in hdt)
            {
                var oldvl = "";
                oldvl = historydata.OldValue;
                var fdt = data.GetType().GetProperty(historydata.FieldName);
                if (fdt != null)
                {
                    var ftp = GetTypeName(fdt.PropertyType);


                    if (ftp == "Int32")
                    {
                        fdt.SetValue(data, Convert.ToInt32(oldvl), null);
                    }
                    if (ftp == "String")
                    {
                        fdt.SetValue(data, oldvl, null);
                    }

                    if (ftp == "long")
                    {
                        fdt.SetValue(data, long.Parse(oldvl), null);
                    }
                    if (ftp == "DateTime")
                    {
                        fdt.SetValue(data, DateTime.Parse(oldvl), null);
                    }
                }
            }
            

            MembersDto m = new MembersDto();
            m.Id = data.Id;
            m.FirstName = data.FirstName;
            m.LastName = data.LastName;
            m.DOB = data.DOB;
            m.Email = data.Email;
            m.BirthPlace = data.BirthPlace;
            m.Nationality = data.Nationality;
            m.LanguageID = data.LanguageID;
            m.Phone = data.Phone;
            m.GSM = data.GSM;
            m.Street = data.Street;
            m.PostalCode = data.PostalCode;
            m.Commune = data.Commune;
            m.PrivateAddress = data.PrivateAddress;
            m.PrivatePostalCode = data.PrivatePostalCode;
            m.FirstNamePublic = data.FirstNamePublic;
            m.LastNamePublic = data.LastNamePublic;
            m.DOBPublic = data.DOBPublic;
            m.EmailPublic = data.EmailPublic;
            m.BirthPlacePublic = data.BirthPlacePublic;
            m.NationalityPublic = data.NationalityPublic;
            m.LanguageIDPublic = data.LanguageIDPublic;
            m.PhonePublic = data.PhonePublic;
            m.GSMPublic = data.GSMPublic;
            m.StreetPublic = data.StreetPublic;
            m.PostalCodePublic = data.PostalCodePublic;
            m.CommunePublic = data.CommunePublic;
            m.PrivateAddressPublic = data.PrivateAddressPublic;
            m.PrivatePostalCodePublic = data.PrivatePostalCodePublic;
            m.EmploymentStatusIDPublic = data.EmploymentStatusIDPublic ?? false;
            m.EmploymentStatusID = (int)data.EmploymentStatusID;
            m.SpecializationPublic = data.SpecializationPublic;
            m.TraineeCommissionPublic = data.TraineeCommissionPublic;

            m.TraneeComissionId = repositoryWrapper.HistoryChangesManager.GetMemberTranieeHistory(pk,date);
            m.Specialization = repositoryWrapper.HistoryChangesManager.GetMemberSpecializationtHistory(pk, date);
            var languages = _ocessor.GetLanguages();
            m.Languages = languages.Select(x => new ListCollectionDto() { Id = x.ID, Value = x.Name }).ToList();
            var employmentstatus = repositoryWrapper.EmploymentStatusManager.GetAll();
            m.EmploymentStatuses = employmentstatus.Select(x => new ListCollectionDto() { Id = (int)x.Id, Value = x.StatusName }).ToList();
            var traningcourse = repositoryWrapper.TraineeCourseManager.GetAll();
            m.TranieeCommission = traningcourse.Select(x => new ListCollectionDto() { Id = (int)x.ID, Value = x.TrainingName }).Where(x => m.TraneeComissionId.Contains((int)x.Id)).ToList();
            return m;
        }
    }
}
