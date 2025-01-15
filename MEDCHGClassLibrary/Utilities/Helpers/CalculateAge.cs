using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities.Helpers
{
    public static class CalCulateAge
    {
        public static int AgeCalculator(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today); // Get today's date as DateOnly
            var age = today.Year - birthDate.Year;

            // Adjust if the birthday hasn't occurred yet this year
            if (birthDate > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
