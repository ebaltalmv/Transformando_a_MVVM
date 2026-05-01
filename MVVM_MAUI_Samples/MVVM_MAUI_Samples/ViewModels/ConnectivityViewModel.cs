using CommunityToolkit.Mvvm.ComponentModel;
using SharedResources.Models;

namespace MVVM_MAUI_Samples.ViewModels
{
    public partial class ConnectivityViewModel : ObservableObject
    {
        private readonly ConnectivityModel _model;

        public ConnectivityViewModel(ConnectivityModel model)
        {
            _model = model;
            UpdateModel();
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        public string NetworkAccess
        {
            get => _model.NetworkAccess;
            set => SetProperty(_model.NetworkAccess, value, _model, (m, v) => m.NetworkAccess = v);
        }

        public string ConnectionProfiles
        {
            get => _model.ConnectionProfiles;
            set => SetProperty(_model.ConnectionProfiles, value, _model, (m, v) => m.ConnectionProfiles = v);
        }

        private void UpdateModel()
        {
            NetworkAccess = Connectivity.NetworkAccess.ToString();
            var profiles = string.Empty;
            foreach (var p in Connectivity.ConnectionProfiles)
                profiles += "\n" + p.ToString();
            ConnectionProfiles = profiles;
        }

        void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            UpdateModel();
        }
    }
}
