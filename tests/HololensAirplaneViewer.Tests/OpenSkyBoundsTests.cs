using HololensAirplaneViewer.Services;
using Xunit;

namespace HololensAirplaneViewer.Tests
{
    public class OpenSkyBoundsTests
    {
        [Fact]
        public void Around_ClampsNorthPoleLatitude()
        {
            var result = OpenSkyBounds.Around(89.5, 10.0, 3.0);

            Assert.Single(result);
            Assert.Equal(86.5, result[0].Lamin, 6);
            Assert.Equal(90.0, result[0].Lamax, 6);
            Assert.Equal(7.0, result[0].Lomin, 6);
            Assert.Equal(13.0, result[0].Lomax, 6);
        }

        [Fact]
        public void Around_ClampsSouthPoleLatitude()
        {
            var result = OpenSkyBounds.Around(-89.5, 10.0, 3.0);

            Assert.Single(result);
            Assert.Equal(-90.0, result[0].Lamin, 6);
            Assert.Equal(-86.5, result[0].Lamax, 6);
        }

        [Fact]
        public void Around_SplitsEastAntimeridian()
        {
            var result = OpenSkyBounds.Around(0.0, 179.0, 3.0);

            Assert.Equal(2, result.Count);
            Assert.Equal(176.0, result[0].Lomin, 6);
            Assert.Equal(180.0, result[0].Lomax, 6);
            Assert.Equal(-180.0, result[1].Lomin, 6);
            Assert.Equal(-178.0, result[1].Lomax, 6);
        }

        [Fact]
        public void Around_SplitsWestAntimeridian()
        {
            var result = OpenSkyBounds.Around(0.0, -179.0, 3.0);

            Assert.Equal(2, result.Count);
            Assert.Equal(178.0, result[0].Lomin, 6);
            Assert.Equal(180.0, result[0].Lomax, 6);
            Assert.Equal(-180.0, result[1].Lomin, 6);
            Assert.Equal(-176.0, result[1].Lomax, 6);
        }

        [Fact]
        public void Around_UsesSingleBoxForNormalLongitude()
        {
            var result = OpenSkyBounds.Around(59.9, 10.7, 3.0);

            Assert.Single(result);
            Assert.Equal(56.9, result[0].Lamin, 6);
            Assert.Equal(62.9, result[0].Lamax, 6);
            Assert.Equal(7.7, result[0].Lomin, 6);
            Assert.Equal(13.7, result[0].Lomax, 6);
        }
    }
}