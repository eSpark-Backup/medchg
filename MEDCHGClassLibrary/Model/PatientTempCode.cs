namespace MEDCHGClassLibrary.Model
{
    public class PatientTempCode:BaseEntity
    {
        public string Name { get; set; }
        public bool IsDiagCode { get; set; }
        public int UserId { get; set; }
        public int PatientVisitId { get; set; }
        public bool IsCurrentlyAdd { get; set; }
        public int? PatientVisitHistoryId { get; set; }
    }
}