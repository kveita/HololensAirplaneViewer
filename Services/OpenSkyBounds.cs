using System;
using System.Collections.Generic;

namespace HololensAirplaneViewer.Services
{
    public struct OpenSkyQueryBounds
    {
        public double Lamin { get; private set; }
        public double Lamax { get; private set; }
        public double Lomin { get; private set; }
        public double Lomax { get; private set; }

        public OpenSkyQueryBounds(double lamin, double lamax, double lomin, double lomax)
        {
            Lamin = lamin;
            Lamax = lamax;
            Lomin = lomin;
            Lomax = lomax;
        }
    }

    /// <summary>
    /// Creates valid OpenSky geographic query boxes around an observer.
    /// A box crossing ±180° is split because OpenSky expects lomin <= lomax
    /// within the [-180°, 180°] longitude range.
    /// </summary>
    public static class OpenSkyBounds
    {
        private const double MinLatitude = -90.0;
        private const double MaxLatitude = 90.0;
        private const double MinLongitude = -180.0;
        private const double MaxLongitude = 180.0;

        public static List<OpenSkyQueryBounds> Around(
            double latitude,
            double longitude,
            double radiusDegrees)
        {
            if (double.IsNaN(latitude)
                || double.IsInfinity(latitude)
                || double.IsNaN(longitude)
                || double.IsInfinity(longitude))
            {
                throw new ArgumentOutOfRangeException("latitude/longitude");
            }

            if (radiusDegrees < 0.0
                || radiusDegrees > 180.0
                || double.IsNaN(radiusDegrees)
                || double.IsInfinity(radiusDegrees))
            {
                throw new ArgumentOutOfRangeException(nameof(radiusDegrees));
            }

            latitude = Clamp(latitude, MinLatitude, MaxLatitude);
            longitude = NormalizeLongitude(longitude);

            double lamin = Clamp(latitude - radiusDegrees, MinLatitude, MaxLatitude);
            double lamax = Clamp(latitude + radiusDegrees, MinLatitude, MaxLatitude);
            double lomin = longitude - radiusDegrees;
            double lomax = longitude + radiusDegrees;

            var result = new List<OpenSkyQueryBounds>();
            if (lomin < MinLongitude)
            {
                result.Add(new OpenSkyQueryBounds(
                    lamin, lamax, lomin + 360.0, MaxLongitude));
                result.Add(new OpenSkyQueryBounds(
                    lamin, lamax, MinLongitude, lomax));
            }
            else if (lomax > MaxLongitude)
            {
                result.Add(new OpenSkyQueryBounds(
                    lamin, lamax, lomin, MaxLongitude));
                result.Add(new OpenSkyQueryBounds(
                    lamin, lamax, MinLongitude, lomax - 360.0));
            }
            else
            {
                result.Add(new OpenSkyQueryBounds(lamin, lamax, lomin, lomax));
            }

            return result;
        }

        private static double NormalizeLongitude(double longitude)
        {
            // Use modulo instead of repeated +/- 360 to avoid unbounded
            // iteration for very large inputs (e.g., 1e300).
            // C# % is remainder (sign follows dividend), so we adjust.
            longitude = longitude % 360.0;
            if (longitude < MinLongitude)
            {
                longitude += 360.0;
            }
            else if (longitude > MaxLongitude)
            {
                longitude -= 360.0;
            }
            return longitude;
        }

        private static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}