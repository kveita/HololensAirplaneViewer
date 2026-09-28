using System;
using System.Globalization;
using HololensAirplaneViewer.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HololensAirplaneViewer
{
    public sealed partial class LocationInputDialog : ContentDialog
    {
        public double FinalLatitude { get; private set; }
        public double FinalLongitude { get; private set; }

        public LocationInputDialog()
        {
            InitializeComponent();
        }

        private void LocationInputDialog_PrimaryButtonClick(
            ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            string geohash = (GeohashTextBox.Text ?? string.Empty).Trim();
            string latitudeText = (LatitudeTextBox.Text ?? string.Empty).Trim();
            string longitudeText = (LongitudeTextBox.Text ?? string.Empty).Trim();

            try
            {
                if (geohash.Length > 0)
                {
                    if (latitudeText.Length > 0 || longitudeText.Length > 0)
                    {
                        Reject(args, "Use either a geohash or latitude and longitude, not both.");
                        return;
                    }

                    GeohashCoordinate coordinate = GeohashConverter.Decode(geohash);
                    FinalLatitude = coordinate.Latitude;
                    FinalLongitude = coordinate.Longitude;
                }
                else
                {
                    double latitude;
                    double longitude;
                    if (!TryParseCoordinate(latitudeText, -90.0, 90.0, out latitude))
                    {
                        Reject(args, "Latitude must be a number between -90 and 90.");
                        return;
                    }

                    if (!TryParseCoordinate(longitudeText, -180.0, 180.0, out longitude))
                    {
                        Reject(args, "Longitude must be a number between -180 and 180.");
                        return;
                    }

                    FinalLatitude = latitude;
                    FinalLongitude = longitude;
                }

                ErrorText.Text = string.Empty;
                ErrorText.Visibility = Visibility.Collapsed;
            }
            catch (ArgumentException)
            {
                Reject(args, "The geohash is invalid. Use letters and numbers from a standard geohash.");
            }
        }

        private static bool TryParseCoordinate(
            string text,
            double minimum,
            double maximum,
            out double value)
        {
            bool parsed = double.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value);

            if (!parsed)
            {
                parsed = double.TryParse(
                    text,
                    NumberStyles.Float,
                    CultureInfo.CurrentCulture,
                    out value);
            }

            return parsed
                && !double.IsNaN(value)
                && !double.IsInfinity(value)
                && value >= minimum
                && value <= maximum;
        }

        private void Reject(ContentDialogButtonClickEventArgs args, string message)
        {
            args.Cancel = true;
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}