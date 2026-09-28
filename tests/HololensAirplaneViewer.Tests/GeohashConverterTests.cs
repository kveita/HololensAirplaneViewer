using HololensAirplaneViewer.Utilities;
using System;
using Xunit;

namespace HololensAirplaneViewer.Tests
{
    public class GeohashConverterTests
    {
        [Fact]
        public void Decode_KnownGeohash_ReturnsCellCenter()
        {
            GeohashCoordinate coordinate = GeohashConverter.Decode("u4pruyd");

            Assert.Equal(57.64870, coordinate.Latitude, 4);
            Assert.Equal(10.40749, coordinate.Longitude, 4);
        }

        [Fact]
        public void Decode_IsCaseInsensitiveAndTrimsWhitespace()
        {
            GeohashCoordinate coordinate = GeohashConverter.Decode(" U4PRUYD ");

            Assert.Equal(57.64870, coordinate.Latitude, 4);
            Assert.Equal(10.40749, coordinate.Longitude, 4);
        }

        [Fact]
        public void Decode_InvalidCharacter_Throws()
        {
            Assert.Throws<ArgumentException>(() => GeohashConverter.Decode("u4pruyi"));
        }

        [Fact]
        public void Decode_EmptyValue_Throws()
        {
            Assert.Throws<ArgumentException>(() => GeohashConverter.Decode(" "));
        }
    }
}