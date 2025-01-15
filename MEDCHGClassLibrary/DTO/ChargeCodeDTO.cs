using MEDCHGClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.DTO
{
    public record ChargeCodeDTO
    {
        public int? Id { get; set; }
        public string? ChargeCodeName { get; set; }
        public string? ChargeCodeDescription { get; set; }
        public int? ProviderId { get; set; }
        public int? CreatedById { get; set; }

    }
}
