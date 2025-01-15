namespace MEDCHGClassLibrary.Model
{
    public class Hospital : BaseEntity
    {
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public string? HsCode { get; set; }
        public string? HospitalName { get; set; }
        public bool IsFavourite { get; set; }
        public List<Building?>? Buildings { get; set; }
        public List<UsersHospital> UsersHospitals { get; set; } = new List<UsersHospital>();
    }
}