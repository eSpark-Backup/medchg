using MEDCHGClassLibrary.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Model
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public int IsActive { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = TimeHelper.GetTexasTime();
        public DateTime Date { get; set; } = TimeHelper.GetTexasTime();
        public int? CreatedById { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedById { get; set; }
        public string? Device { get; set; }
    }
}
