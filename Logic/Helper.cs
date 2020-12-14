using System;
using System.Runtime.InteropServices;

namespace Logic
{
    public static class Helper
    {
        public static DateTime GetDateTimeZone()
        {
            TimeZoneInfo easternTimeZone = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                easternTimeZone = TimeZoneInfo.Local;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Tucuman");
            }
            else
            {
                throw new ArgumentException("Not supported OS");
            }
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternTimeZone);
        }
    }
}
