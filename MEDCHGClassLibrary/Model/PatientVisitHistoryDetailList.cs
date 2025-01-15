using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Model
{
    public enum CensusStatus 
    { 
        ADMITTED = 1,
        DISCHARGED,
        BILLED
    }
    public class PatientVisitHistoryDetailList:BaseEntity
    {
        public string? CodeName { get; set; }
        public string? CodeDescription { get; set; }
        public string? CodeType { get; set; }
        public string? CodeSubType { get; set; }
        public string? Note { get; set; }
        public string? PractitionerName { get; set; }
        public int? RoomId { get; set; }
        public int? FloorId { get; set; }
        public int? BuildingId { get; set; }
        public int? HospitalId { get; set; }
        public int? PractitionerId { get; set; }
        public int? PatientVisitHistoryId { get; set; }
    }
}
