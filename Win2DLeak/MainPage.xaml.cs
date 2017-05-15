using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.ApplicationModel.Core;
using Windows.Devices.Sensors;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win2DLeak {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Compass compass;

        public MainPage()
        {
            this.InitializeComponent();

            CanvasCtrl.Draw += OnDraw;
            SizeChanged += OnSizeChanged;
            CanvasCtrl.Loaded += OnLoaded;
            CanvasCtrl.Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            compass = Compass.GetDefault();
            if(compass != null) {
                uint minReportInterval = compass.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                compass.ReportInterval = reportInterval;

                compass.ReadingChanged += OnCompassReadingChanged;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) {
            if(compass != null) {
                compass.ReadingChanged -= OnCompassReadingChanged;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
            double newWidth = e.NewSize.Width;
            double newHeight = e.NewSize.Height;

            double size = newWidth >= newHeight ? newHeight : newWidth;

            CanvasCtrl.Width = size;
            CanvasCtrl.Height = size;
        }

        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args) {
            args.DrawingSession.DrawCircle(new System.Numerics.Vector2(((float) sender.ActualWidth / 2), (float) (sender.ActualHeight / 2)), 100f, Color.FromArgb(255, 255, 0, 255));
        }

        private async void OnCompassReadingChanged(Compass sender, CompassReadingChangedEventArgs args) {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                CompassReading reading = args.Reading;
                if(reading.HeadingTrueNorth.HasValue) {
                    CanvasCtrl.Invalidate();
                }
            });
        }
    }
}
