using System.Collections.Generic;

namespace SharedResources.Models
{
    public class QRScannerModel
    {
        public double ToleranceMeters { get; set; } = 50.0;
        public List<AccessPoint> AccessPoints { get; set; } = new List<AccessPoint>();

        public QRScannerModel()
        {
            // Puntos de acceso predeterminados
            AccessPoints.Add(new AccessPoint { Id = "AP01", Name = "Entrada Principal", Latitude = 19.0494, Longitude = -98.2036 });
            AccessPoints.Add(new AccessPoint { Id = "AP02", Name = "Estacionamiento Sur", Latitude = 19.0480, Longitude = -98.2040 });
        }
    }
}
