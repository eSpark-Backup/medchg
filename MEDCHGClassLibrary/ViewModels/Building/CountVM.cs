using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.ViewModels.Building
{
    public class CountVM
    {
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public List<BuildingDetailVM> Buildings { get; set; }
    }
    public class BuildingDetailVM
    {
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }  // Added BuildingName
        public string BuildingLocation { get; set; }
        public int FloorCount { get; set; }
        public List<FloorDetailVM> Floors { get; set; }
    }
    public class FloorDetailVM
    {
        public int FloorId { get; set; }
        public int RoomCount { get; set; }
    }
}
