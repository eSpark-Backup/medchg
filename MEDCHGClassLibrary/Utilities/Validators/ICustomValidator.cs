using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities.Validators
{
    public  interface ICustomValidator
    {
           bool IsValid(object value);

        string ErrorMessage { get; set; }
    }
}
