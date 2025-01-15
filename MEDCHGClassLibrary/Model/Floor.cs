using System.ComponentModel.DataAnnotations;

namespace MEDCHGClassLibrary.Model
{
    public class Floor:BaseEntity
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Floor Name must be between {2} and {1} characters long.")]
        public string Name { get; set; }
        public int BuildingId { get; set; }

        //[StringLength(5, MinimumLength = 2, ErrorMessage = "Room Code must be between {2} and {1} characters long.")]

        public string? RoomCode { get; set; }


        public string? RoomRange { get; set; } /*{ get => RoomRange1 + "-" + RoomRange2; set => RoomRange = value; } */
        public int RoomRange1 { get; set; }
        public int RoomRange2 { get; set; }
        public string? description { get; set; }

        public Room? Room { get; set; }
        public List<Room>? Rooms { get; set; }

        public IEnumerable<string>? AlreadyCreated { get; set; }
    }
}