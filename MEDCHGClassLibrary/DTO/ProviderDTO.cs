using System.ComponentModel.DataAnnotations;

namespace MEDCHGClassLibrary.DTO
{
    public class ProviderDTO
    {
        public int? Id { get; set; }
        public int? CreatedAndUpdatedById { get; set; }
        public int? IsActive { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string DisplayName { get; set; } = null!;
        
    }
}
