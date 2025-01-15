using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Model
{
    public class UsersChargeCode:BaseEntity
    {
        public int UserId { get; set; }
        public int ChargeCodeId { get; set; }
        public bool IsFavourite { get; set; }
    }
    [NotMapped]
    public class UsersChargeCodeList
    {
        public int UserId { get; set; }
        public int ChargeCodeId { get; set; }
        public bool IsFavourite { get; set; }
    }
}
