//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace webIEA.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class MemberProfile
    {
        public MemberProfile()
        {
            this.MemberDocuments = new HashSet<MemberDocument>();
            this.MemberSpecializations = new HashSet<MemberSpecialization>();
            this.MemberTranieeCommissions = new HashSet<MemberTranieeCommission>();
        }
    
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
        public string LanguageID { get; set; }
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
        public Nullable<int> StatusID { get; set; }
        public bool StatusIDPublic { get; set; }
        public System.DateTime AddedOn { get; set; }
        public long AddedBy { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public long ModifiedBy { get; set; }
        public Nullable<int> EmploymentStatusID { get; set; }
    
        public virtual ICollection<MemberDocument> MemberDocuments { get; set; }
        public virtual MemberStatu MemberStatu { get; set; }
        public virtual ICollection<MemberSpecialization> MemberSpecializations { get; set; }
        public virtual ICollection<MemberTranieeCommission> MemberTranieeCommissions { get; set; }
    }
}
