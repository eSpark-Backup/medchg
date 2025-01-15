using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MEDCHGClassLibrary.Model
{
    public class Building : BaseEntity
    {
        [Required]
        [StringLength(120, MinimumLength = 1, ErrorMessage = "Building Name must be between {2} and {1} characters long.")]

        [Column("BuildingName")]
        public string Name { get; set; }

        public string BuildingLocation { get; set; } = " ";

        // [Required]
        [Range(1, 10, ErrorMessage = "Floors must be berween than 1 and 10")]

        public int? NoOfFloors { get; set; }

        public int FloorRange1 { get; set; }

        public int FloorRange2 { get; set; }
        public List<Floor>? Floors { get; set; }
        public int HospitalId { get; set; }
    }
}