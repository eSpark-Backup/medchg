using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Model
{
    public class DeleteLog 
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public int DeletedRowId { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedById { get; set; }
    }
}
