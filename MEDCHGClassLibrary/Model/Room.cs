using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MEDCHGClassLibrary.Model
{
    public class Room:BaseEntity,INotifyPropertyChanged
    {

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(RoomCode))
                {
                    return RoomNo;
                }
                else
                {
                    return $"{RoomCode}-{RoomNo}";
                }
            }
            set
            {
                if (value.Contains("-"))
                {
                    var parts = value.Split('-');
                    RoomCode = parts[0];
                    RoomNo = parts[1];
                }
                else
                {
                    RoomCode = null;
                    RoomNo = value;
                }
            }
        }

        //[Range(3, 120, ErrorMessage = "Value must be between {1} and {2}\")")]

        //[StringLength(5, MinimumLength = 2, ErrorMessage = "Name must be between {2} and {1} characters long.")]
        public string? RoomCode { get; set; }


        [Required]
        //  [Range(1,int.MaxValue, ErrorMessage = "Room No must be greate than 0")]
        public string? RoomNo { get; set; }
        public string? Description { get; set; }
        public int FloorId { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Patient? Patient { get; set; }

    }
}