using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using HololensAirplaneViewer.Services;

namespace HololensAirplaneViewer
{
    public sealed partial class SettingsPage : Page
    {
        private bool dialogShown;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (dialogShown)
            {
                return;
            }

            dialogShown = true;

            try
            {
                var dialog = new LocationInputDialog();
                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    LocationOverrideStore.Set(dialog.FinalLatitude, dialog.FinalLongitude);
                }
            }
            finally
            {
                Window.Current.Close();
            }
        }
    }
}
