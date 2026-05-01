using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharedResources.Models;

namespace MVVM_MAUI_Samples.ViewModels
{
    public partial class GeolocationViewModel : ObservableObject
    {
        private readonly GeolocationModel _model;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        private CancellationTokenSource cts;

        public GeolocationViewModel(GeolocationModel model)
        {
            _model = model;
        }

        public string LastLocation
        {
            get => _model.LastLocation;
            set => SetProperty(_model.LastLocation, value, _model, (m, v) => m.LastLocation = v);
        }

        public string CurrentLocation
        {
            get => _model.CurrentLocation;
            set => SetProperty(_model.CurrentLocation, value, _model, (m, v) => m.CurrentLocation = v);
        }

        public int Accuracy
        {
            get => _model.Accuracy;
            set => SetProperty(_model.Accuracy, value, _model, (m, v) => m.Accuracy = v);
        }

        public string[] Accuracies
        {
            get => _model.Accuracies;
            set => SetProperty(_model.Accuracies, value, _model, (m, v) => m.Accuracies = v);
        }

        public bool IsListening
        {
            get => _model.IsListening;
            set => SetProperty(_model.IsListening, value, _model, (m, v) => m.IsListening = v);
        }

        public bool IsNotListening
        {
            get => _model.IsNotListening;
            set => SetProperty(_model.IsNotListening, value, _model, (m, v) => m.IsNotListening = v);
        }

        public string ListeningLocation
        {
            get => _model.ListeningLocation;
            set => SetProperty(_model.ListeningLocation, value, _model, (m, v) => m.ListeningLocation = v);
        }

        public string ListeningLocationStatus
        {
            get => _model.ListeningLocationStatus;
            set => SetProperty(_model.ListeningLocationStatus, value, _model, (m, v) => m.ListeningLocationStatus = v);
        }

        [RelayCommand]
        async Task GetLastLocationAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                LastLocation = FormatLocation(location);
            }
            catch (Exception ex)
            {
                LastLocation = FormatLocation(null, ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task GetCurrentLocationAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var request = new GeolocationRequest((GeolocationAccuracy)Accuracy);
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);
                CurrentLocation = FormatLocation(location);
            }
            catch (Exception ex)
            {
                CurrentLocation = FormatLocation(null, ex);
            }
            finally
            {
                if (cts != null)
                {
                    cts.Dispose();
                    cts = null;
                }
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task StartListeningAsync()
        {
            try
            {
                Geolocation.LocationChanged += Geolocation_LocationChanged;
                var request = new GeolocationListeningRequest((GeolocationAccuracy)Accuracy);
                var success = await Geolocation.StartListeningForegroundAsync(request);

                ListeningLocationStatus = success
                    ? "Started listening for foreground location updates"
                    : "Couldn't start listening";
            }
            catch (Exception ex)
            {
                ListeningLocationStatus = FormatLocation(null, ex);
            }

            IsListening = Geolocation.IsListeningForeground;
            IsNotListening = !IsListening;
        }

        void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            ListeningLocation = FormatLocation(e.Location);
        }

        [RelayCommand]
        void StopListening()
        {
            try
            {
                Geolocation.LocationChanged -= Geolocation_LocationChanged;
                Geolocation.StopListeningForeground();
                ListeningLocationStatus = "Stopped listening for foreground location updates";
            }
            catch (Exception ex)
            {
                ListeningLocationStatus = FormatLocation(null, ex);
            }

            IsListening = Geolocation.IsListeningForeground;
            IsNotListening = !IsListening;
        }

        string FormatLocation(Location location, Exception ex = null)
        {
            if (location == null)
            {
                return $"Unable to detect location. Exception: {ex?.Message ?? string.Empty}";
            }
            string notAvailable = "not available";
            return
                $"Latitude: {location.Latitude}\n" +
                $"Longitude: {location.Longitude}\n" +
                $"HorizontalAccuracy: {location.Accuracy}\n" +
                $"Altitude: {(location.Altitude.HasValue ? location.Altitude.Value.ToString() : notAvailable)}\n" +
                $"AltitudeRefSys: {location.AltitudeReferenceSystem.ToString()}\n" +
                $"VerticalAccuracy: {(location.VerticalAccuracy.HasValue ? location.VerticalAccuracy.Value.ToString() : notAvailable)}\n" +
                $"Heading: {(location.Course.HasValue ? location.Course.Value.ToString() : notAvailable)}\n" +
                $"Speed: {(location.Speed.HasValue ? location.Speed.Value.ToString() : notAvailable)}\n" +
                $"Date (UTC): {location.Timestamp:d}\n" +
                $"Time (UTC): {location.Timestamp:T}\n" +
                $"Mocking Provider: {location.IsFromMockProvider}";
        }

        public void CancelOperations()
        {
            if (IsBusy && cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            StopListening();
        }
    }
}
