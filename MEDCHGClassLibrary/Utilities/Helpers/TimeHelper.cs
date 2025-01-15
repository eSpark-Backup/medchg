using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities.Helpers
{
    public static class TimeHelper
    {
        public static DateTime GetTexasTime()
        {
            // Central Standard Time zone (Texas)
            var texasTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            // Get the current time in UTC
            DateTime utcNow = DateTime.UtcNow;

            // Convert UTC to Texas time (CST/CDT depending on the time of year)
            DateTime texasTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, texasTimeZone);

            return texasTime;
        }
    }
}
