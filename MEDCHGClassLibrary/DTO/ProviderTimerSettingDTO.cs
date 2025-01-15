namespace MEDCHGClassLibrary.DTO
{
    public record ProviderTimerSettingDTO
    {
        public int? Id { get; set; }
        public int? ProviderId { get; set; }
        public string? RedInDays { get; set; }
        public string? GreenInDays { get; set; }
        public string? YellowInDays { get; set; }

    }
}
