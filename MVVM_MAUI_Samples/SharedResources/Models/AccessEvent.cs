namespace SharedResources.Models
{
    public class AccessEvent
    {
        public DateTime Timestamp { get; set; }

        public string QRIdentifier { get; set; } = string.Empty;

        public string AccessPointId { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public double ScannerLatitude { get; set; }

        public double ScannerLongitude { get; set; }

        public string CapturedPhotoPath { get; set; } = string.Empty;

        public string NetworkStatus { get; set; } = string.Empty;

        public string ConnectionProfile { get; set; } = string.Empty;
    }
}
