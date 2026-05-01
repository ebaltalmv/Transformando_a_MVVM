using System;
using Microsoft.Maui.Devices.Sensors;

namespace SharedResources.Models
{
    public class GeolocationModel
    {
        public string LastLocation { get; set; } = string.Empty;
        public string CurrentLocation { get; set; } = string.Empty;
        public int Accuracy { get; set; } = (int)GeolocationAccuracy.Default;
        public string[] Accuracies { get; set; } = Enum.GetNames(typeof(GeolocationAccuracy));
        public bool IsListening { get; set; }
        public bool IsNotListening { get; set; } = true;
        public string ListeningLocation { get; set; } = string.Empty;
        public string ListeningLocationStatus { get; set; } = string.Empty;
    }
}
