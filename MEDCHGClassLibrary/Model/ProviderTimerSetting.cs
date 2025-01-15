using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Model
{
    public class ProviderTimerSetting : BaseEntity
    {
        public int ProviderId { get; set; }
        public string? RedInDays { get; set; }
        public string? GreenInDays { get; set; }
        public string? YellowInDays { get; set; }

    }
}
