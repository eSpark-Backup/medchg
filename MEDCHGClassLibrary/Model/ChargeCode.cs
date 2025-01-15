namespace MEDCHGClassLibrary.Model
{
    public class ChargeCode : BaseEntity
    {
        public string ChargeCodeName { get; set; }
        public string? ChargeCodeDescription { get; set; }
        public bool IsFavourite { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
    }
}