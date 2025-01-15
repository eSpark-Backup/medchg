using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Model
{
    public class Provider : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string SubscriptionId { get; set; } = null!;
        public List<User?>? User { get; set; }
        public List<DiagnosticCode?>? DiagnosticCode { get; set; }
        public List<ChargeCode?>? ChargeCode { get; set; }
        public ICollection<Hospital?>? Hospitals { get; }

        public Provider()
        {
            // Generate SubscriptionId automatically
            SubscriptionId = "SB-" + GetNextSubscriptionId();
        }

        private static int lastSubscriptionId = 0;
        private static string GetNextSubscriptionId()
        {
            lastSubscriptionId++;
            return lastSubscriptionId.ToString("D3"); 
        }
    }
}
