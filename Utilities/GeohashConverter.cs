using System;

namespace HololensAirplaneViewer.Utilities
{
    /// <summary>
    /// Decodes standard base32 geohashes without network access or external
    /// dependencies. The returned point is the center of the geohash cell.
    /// </summary>
    public static class GeohashConverter
    {
        private const string Base32 = "0123456789bcdefghjkmnpqrstuvwxyz";

        public static GeohashCoordinate Decode(string geohash)
        {
            if (string.IsNullOrWhiteSpace(geohash))
            {
                throw new ArgumentException("Geohash code cannot be empty.", "geohash");
            }

            geohash = geohash.Trim().ToLowerInvariant();

            bool isEvenBit = true;
            double[] latitude = { -90.0, 90.0 };
            double[] longitude = { -180.0, 180.0 };

            foreach (char character in geohash)
            {
                int value = Base32.IndexOf(character);
                if (value < 0)
                {
                    throw new ArgumentException(
                        string.Format("Invalid geohash character: {0}", character),
                        "geohash");
                }

                for (int mask = 16; mask > 0; mask >>= 1)
                {
                    double[] interval = isEvenBit ? longitude : latitude;
                    if ((value & mask) != 0)
                    {
                        interval[0] = (interval[0] + interval[1]) / 2.0;
                    }
                    else
                    {
                        interval[1] = (interval[0] + interval[1]) / 2.0;
                    }

                    isEvenBit = !isEvenBit;
                }
            }

            return new GeohashCoordinate(
                (latitude[0] + latitude[1]) / 2.0,
                (longitude[0] + longitude[1]) / 2.0);
        }
    }

    public sealed class GeohashCoordinate
    {
        public GeohashCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
    }
}