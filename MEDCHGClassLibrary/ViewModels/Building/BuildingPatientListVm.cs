using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.ViewModels.Building;

public class BuildingPatientListVm
{
    public BuildingPatientListVm()
    {
        Floors = new List<FloorList>();
    }
    public int BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public string? BuildingLocation { get; set; }
    public int? PatientCount { get; set; }
    public int? FloorCount { get; set; }
    public List<FloorList> Floors { get; set; }

}

public class FloorList
{
    public FloorList()
    {
        Patients = new List<PatientList>();
    }
    public string? FloorName { get; set; }
    public int? FloorId { get; set; }
    public List<PatientList> Patients { get; set; }
}

public class PatientList
{
    public int PatientId { get; set; }
    public string? FullName { get; set; }
    public string? MRN { get; set; }
    public int Gender { get; set; }
    public int? UserId { get; set; }
    public int? IsActive { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? RoomName { get; set; }
    public int? RoomId { get; set; }
}