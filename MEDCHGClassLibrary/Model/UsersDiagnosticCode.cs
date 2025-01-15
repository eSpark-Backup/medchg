using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Model
{
    public class UsersDiagnosticCode :BaseEntity
    {
        public int UserId { get; set; }
        public int DiagnosticCodeId { get; set; }
        public bool IsFavourite { get; set; }

    }
}
