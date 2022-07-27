using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Dtos
{
    public class MembersDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public bool FirstNamePublic { get; set; }
        public string LastName { get; set; }
        public bool LastNamePublic { get; set; }
        public string Email { get; set; }
        public bool EmailPublic { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public bool DOBPublic { get; set; }
        public string BirthPlace { get; set; }
        public bool BirthPlacePublic { get; set; }
        public string Nationality { get; set; }
        public bool NationalityPublic { get; set; }
        public int LanguageID { get; set; }
        public bool LanguageIDPublic { get; set; }
        public string Phone { get; set; }
        public bool PhonePublic { get; set; }
        public string GSM { get; set; }
        public bool GSMPublic { get; set; }
        public string Street { get; set; }
        public bool StreetPublic { get; set; }
        public string PostalCode { get; set; }
        public bool PostalCodePublic { get; set; }
        public string Commune { get; set; }
        public bool CommunePublic { get; set; }
        public string PrivateAddress { get; set; }
        public bool PrivateAddressPublic { get; set; }
        public string PrivatePostalCode { get; set; }
        public bool PrivatePostalCodePublic { get; set; }
        public int StatusID { get; set; }
        public bool StatusIDPublic { get; set; }
        public System.DateTime AddedOn { get; set; }
        public long AddedBy { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public long ModifiedBy { get; set; }
        public List<ListCollectionDto> Specialization { get; set; }
        public List<ListCollectionDto> TranieeCommission { get; set; }
    }

    public class RequestMemberDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string BirthPlace { get; set; }
        public string Nationality { get; set; }
        public int LanguageID { get; set; }
        public string Phone { get; set; }
        public string GSM { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Commune { get; set; }
        public string PrivateAddress { get; set; }
        public string PrivatePostalCode { get; set; }
        public int? StatusID { get; set; }
        public List<ListCollectionDto> Specialization { get; set; }
        public List<ListCollectionDto> TranieeCommission { get; set; }
    } 
}
