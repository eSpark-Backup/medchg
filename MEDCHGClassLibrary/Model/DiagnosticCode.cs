namespace MEDCHGClassLibrary.Model
{
    public class DiagnosticCode : BaseEntity
    {
        public string DiagnosticCodeName { get; set; }
        public string? DiagnosticCodeDescription { get; set; }
        public bool IsFavourite { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
    }
}