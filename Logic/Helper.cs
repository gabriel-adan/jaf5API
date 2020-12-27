using SharpArch.Domain.DomainModel;
using System;
using System.Collections;
using System.Collections.Generic;
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
                throw new Exception("Not supported OS");
            }
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternTimeZone);
        }

        public static void ThrowIfIsNullOrEmpty(string param, string message)
        {
            if (string.IsNullOrEmpty(param))
                throw new ArgumentException(message);
        }

        public static void ThrowIfNull(Entity entity, string message)
        {
            if (entity == null)
                throw new ArgumentException(message);
        }

        public static void ThrowIfExists(Entity entity, string message)
        {
            if (entity != null)
                throw new ArgumentException(message);
        }

        public static void ThrowIf(bool condition, string message)
        {
            if (condition)
                throw new ArgumentException(message);
        }

        public static void ThrowIfIsNullOrEmpty<T>(IList<T> list, string message)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException(message);
        }
    }
}
