using System.ComponentModel.DataAnnotations.Schema;

namespace MEDCHGClassLibrary.Model
{
    public class PatientVisitHistory : BaseEntity
    {
        public int? PatientId { get; set; }
        public int? ProviderId { get; set; }
        public DateTime? PatientAddmissionDate { get; set; }
        public DateTime? PatientDischargeDate { get; set; }
        public int? PatientDischargeById { get; set; }
        public DateTime? PatientBilledDate { get; set; }
        public int? PatientBilledById { get; set; }
        public DateTime? PatientAddmissionTime { get; set; }
        public int? PatientAge { get; set; }
        public CensusStatus? PatientCensusStatus { get; set; }
        public int? UserId { get; set; }
        [NotMapped]
        public List<PatientVisitHistoryDetailList>? PatientVisitHistoryDetailLists { get; set; } = new();
    }
}
