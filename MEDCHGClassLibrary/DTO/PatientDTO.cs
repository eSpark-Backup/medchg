using MEDCHGClassLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.DTO
{
    public class PatientDTO
    {
        public string? MRNNumber { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Salutation { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? DOB { get; set; }
        public DateTime? PatientAddmissionDateTime { get; set; }
        public int? UserId { get; set; }
        public int? BuildingId { get; set; }
        public int? FloorId { get; set; }
        public int? RoomId { get; set; }
        public int? PateintId { get; set; }
        public int? ProviderId { get; set; }
        public int? HospitalId { get; set; }
        public string? BuildingName { get; set; }
        public string? FloorName { get; set; }
        public string? RoomName { get; set; }
        public string? PateintName { get; set; }
        public string? HospitalName { get; set; }
        public string? PractitionerName { get; set; }
        public string? Device { get; set; }
        public int? CreatedUserId { get; set; }
        public int? PatientCensusStatus { get; set; }
        public int? PatientId { get; set; }
        public DateTime? PatientAddmissionDate { get; set; }
        public DateTime? PatientDischargeDate { get; set; }
        public int? PatientDischargeById { get; set; }
        public DateTime? PatientBilledDate { get; set; }
        public int? PatientBilledById { get; set; }
        public DateTime? PatientAddmissionTime { get; set; }
        public int? PatientAge { get; set; }
        public string? CodeName { get; set; }
        public string? CodeDescription { get; set; }
        public string? CodeType { get; set; }
        public string? CodeSubType { get; set; }
        public string? Note { get; set; }
        public int? PractitionerId { get; set; }
        public int? PatientVisitHistoryId { get; set; }
        public CensusStatus? CensusState { get; set; }
        public Patient? PatientObj { get; set; } 
        public PatientVisitHistory? PatientVisitHistoryObj { get; set; }
        public PatientVisitHistoryDetailList? PatientVisitHistoryDetailListObj { get; set; }
        public Building? BuildingObj { get; set; }
        public Floor? FloorObj { get; set; }
        public Room? RoomObj { get; set; }
        public List<Patient?>? PatientList { get; set; }
        public List<PatientVisitHistory?>? PatientVisitHistoryList { get; set; }
        public List<PatientVisitHistoryDetailList?>? PatientVisitHistoryDetailLists { get; set; }
        public List<Building?>? BuildingList { get; set; }
        public List<Floor?>? FloorList { get; set; }
        public List<Room?>? RoomList { get; set; }
    }
}
