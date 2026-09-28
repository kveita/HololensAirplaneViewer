using System;

namespace HololensAirplaneViewer.Services
{
    /// <summary>
    /// Thread-safe bridge between the native XAML settings view and the
    /// Direct3D holographic view. The override remains active until replaced
    /// by another manual location.
    /// </summary>
    public static class LocationOverrideStore
    {
        private static readonly object SyncRoot = new object();
        private static bool hasOverride;
        private static double latitude;
        private static double longitude;

        public static void Set(double newLatitude, double newLongitude)
        {
            lock (SyncRoot)
            {
                latitude = newLatitude;
                longitude = newLongitude;
                hasOverride = true;
            }
        }

        public static bool TryGet(out double currentLatitude, out double currentLongitude)
        {
            lock (SyncRoot)
            {
                currentLatitude = latitude;
                currentLongitude = longitude;
                return hasOverride;
            }
        }
    }
}