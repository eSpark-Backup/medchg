using MEDCHGClassLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.ViewModels.Patient;

public class PatientInfoVm
{
    public PatientInfoVm()
    {
        Visits = new List<PatientVisits>();
    }
    public int? Id { get; set; }
    public string? FullName { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? MRN { get; set; }
    public string? Salutation { get; set; }
    public int? Age { get; set; }
    public int? Gender { get; set; }
    public int? PatientVisitsId { get; set; }
    public int? PatientVisitHistoryId { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Practitioner { get; set; }
    public int? HospitalId { get; set; }
    public string? HospitalName { get; set; }
    public int? BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public int? FloorId { get; set; }
    public string? FloorName { get; set; }
    public int? RoomId { get; set; }
    public string? RoomName { get; set; }
    public int? UserId { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? CensusStatus { get; set; }
    public List<PatientVisits> Visits { get; set; }
}

public class PatientVisits
{
    public int? PatientVisitId { get; set; }
    public int PatientId { get; set; }
    public int PatientVisitHistoryId { get; set; }
    public int PatientVisitHistoryDetailId { get; set; }
    public string? Name { get; set; }
    public string? CodeName { get; set; }
    public string? CodeDescription { get; set; }
    public string? Type { get; set; }
    public string? Username { get; set; }
    public DateTime? CreatedAt { get; set; }

}

