using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharedResources.Models;
using System.Collections.ObjectModel;

namespace MVVM_MAUI_Samples.ViewModels
{
    public partial class QRScannerViewModel : ObservableObject
    {
        private readonly QRScannerModel _qrModel;
        private readonly ConnectivityModel _connectivityModel;
        private readonly MediaPickerModel _mediaModel;

        [ObservableProperty]
        private string qrInput;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotScanning))]
        private bool isScanning;

        public bool IsNotScanning => !IsScanning;

        // Connectivity status exposed to the view
        [ObservableProperty]
        private string networkStatus;

        [ObservableProperty]
        private string connectionProfile;

        // Photo evidence exposed to the view
        [ObservableProperty]
        private string capturedPhotoPath;

        [ObservableProperty]
        private bool showCapturedPhoto;

        public ObservableCollection<AccessEvent> Events { get; } = new ObservableCollection<AccessEvent>();

        public QRScannerViewModel(QRScannerModel qrModel, ConnectivityModel connectivityModel, MediaPickerModel mediaModel)
        {
            _qrModel = qrModel;
            _connectivityModel = connectivityModel;
            _mediaModel = mediaModel;

            // Initialize connectivity state
            UpdateConnectivity();
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        private void UpdateConnectivity()
        {
            var access = Connectivity.NetworkAccess.ToString();
            var profiles = string.Join(", ", Connectivity.ConnectionProfiles.Select(p => p.ToString()));

            _connectivityModel.NetworkAccess = access;
            _connectivityModel.ConnectionProfiles = profiles;

            NetworkStatus = access;
            ConnectionProfile = profiles;
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            UpdateConnectivity();
        }

        [RelayCommand]
        async Task SimulateScanAsync()
        {
            if (string.IsNullOrWhiteSpace(QrInput))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor ingresa un identificador QR.", "OK");
                return;
            }

            IsScanning = true;

            try
            {
                // ── Step 1: Verify Connectivity ──
                UpdateConnectivity();
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    var noNetEvent = CreateEvent(QrInput, "Denegado (Sin Conexión a Internet)");
                    noNetEvent.NetworkStatus = NetworkStatus;
                    noNetEvent.ConnectionProfile = ConnectionProfile;
                    Events.Insert(0, noNetEvent);
                    await Application.Current.MainPage.DisplayAlert("Acceso Denegado", "No hay conexión a internet disponible.", "OK");
                    return;
                }

                // ── Step 2: Capture Photo via Camera ──
                string photoPath = string.Empty;
                try
                {
                    FileResult photo = null;
                    if (MediaPicker.IsCaptureSupported)
                    {
                        photo = await MediaPicker.CapturePhotoAsync();
                    }
                    else
                    {
                        // Fallback: pick photo from gallery if camera is not available
                        photo = await MediaPicker.PickPhotoAsync();
                    }

                    if (photo != null)
                    {
                        var newFile = Path.Combine(FileSystem.CacheDirectory, $"qr_evidence_{DateTime.Now:yyyyMMddHHmmss}_{photo.FileName}");
                        using (var stream = await photo.OpenReadAsync())
                        using (var newStream = File.OpenWrite(newFile))
                        {
                            await stream.CopyToAsync(newStream);
                        }
                        photoPath = newFile;

                        // Update media model
                        _mediaModel.PhotoPath = photoPath;
                        _mediaModel.ShowPhoto = true;
                        _mediaModel.ShowVideo = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al capturar foto: {ex.Message}");
                    // Continue with scan even without photo
                }

                CapturedPhotoPath = photoPath;
                ShowCapturedPhoto = !string.IsNullOrEmpty(photoPath);

                // ── Step 3: Validate Geolocation ──
                var accessEvent = await ProcessGeolocationValidationAsync(QrInput);

                // Attach connectivity and photo info to the event
                accessEvent.NetworkStatus = NetworkStatus;
                accessEvent.ConnectionProfile = ConnectionProfile;
                accessEvent.CapturedPhotoPath = photoPath;

                Events.Insert(0, accessEvent);

                if (accessEvent.Status == "Autorizado")
                {
                    await Application.Current.MainPage.DisplayAlert("Acceso Autorizado",
                        $"Punto de Acceso: {accessEvent.AccessPointId}\nRed: {accessEvent.NetworkStatus}",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Acceso Denegado", accessEvent.Status, "OK");
                }
            }
            finally
            {
                QrInput = string.Empty;
                IsScanning = false;
            }
        }

        private async Task<AccessEvent> ProcessGeolocationValidationAsync(string qrIdentifier)
        {
            Location currentLocation;
            try
            {
                currentLocation = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo ubicación: {ex.Message}");
                return CreateEvent(qrIdentifier, "Denegado (Error de Ubicación)");
            }

            if (currentLocation == null)
            {
                return CreateEvent(qrIdentifier, "Denegado (Ubicación Nula)");
            }

            foreach (var ap in _qrModel.AccessPoints)
            {
                var apLocation = new Location(ap.Latitude, ap.Longitude);
                double distanceMeters = Location.CalculateDistance(currentLocation, apLocation, DistanceUnits.Kilometers) * 1000;

                if (distanceMeters <= _qrModel.ToleranceMeters)
                {
                    return new AccessEvent
                    {
                        Timestamp = DateTime.Now,
                        QRIdentifier = qrIdentifier,
                        AccessPointId = ap.Id,
                        Status = "Autorizado",
                        ScannerLatitude = currentLocation.Latitude,
                        ScannerLongitude = currentLocation.Longitude
                    };
                }
            }

            return new AccessEvent
            {
                Timestamp = DateTime.Now,
                QRIdentifier = qrIdentifier,
                Status = "Denegado (Fuera de Rango)",
                ScannerLatitude = currentLocation.Latitude,
                ScannerLongitude = currentLocation.Longitude
            };
        }

        private AccessEvent CreateEvent(string qrIdentifier, string status)
        {
            return new AccessEvent
            {
                Timestamp = DateTime.Now,
                QRIdentifier = qrIdentifier,
                Status = status,
                ScannerLatitude = 0,
                ScannerLongitude = 0
            };
        }
    }
}
