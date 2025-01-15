namespace MEDCHGClassLibrary.DTO
{
    public record DiagnosticCodeDTO
    {
        public int? Id { get; set; }
        public string? DiagnosticCodeName { get; set; }
        public string? DiagnosticCodeDescription { get; set; }
        public int? ProviderId { get; set; }
        public int? CreatedById { get; set; }

    }
}
