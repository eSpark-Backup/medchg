using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.ViewModels.Hospital;

public class HospitalBuildingListVm
{
    public HospitalBuildingListVm()
    {
        HospitalBuildings = new List<HospitalBuildingList>();
    }
    public int HospitalId { get; set; }
    public string? HospitalName { get; set; }
    public List<HospitalBuildingList> HospitalBuildings { get; set; }
}
public class HospitalBuildingList
{
    public int? Id { get; set; }
    public string? BuildingName { get; set; }
    public string? BuildingLocation { get; set; }
    public int? FloorCount { get; set; }
    public int? PatientCount { get; set; }
}