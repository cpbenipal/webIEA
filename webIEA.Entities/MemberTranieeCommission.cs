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
    
    public partial class MemberTranieeCommission
    {
        public int Id { get; set; }
        public Nullable<long> MemberID { get; set; }
        public Nullable<int> TrainingCourseId { get; set; }
        public string Description { get; set; }
        public Nullable<int> AllocatedHours { get; set; }
        public System.DateTime AddedOn { get; set; }
        public long AddedBy { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public long ModifiedBy { get; set; }
    
        public virtual MemberProfile MemberProfile { get; set; }
        public virtual TrainingCours TrainingCours { get; set; }
    }
}
