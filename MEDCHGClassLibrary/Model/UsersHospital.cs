namespace MEDCHGClassLibrary.Model
{
    public class UsersHospital:BaseEntity
    {
        public int UserId { get; set; }
        public int HospitalId { get; set; }
        public bool IsFavourite { get; set; }
    }
}