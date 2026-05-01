using MVVM_MAUI_Samples.ViewModels;

namespace MVVM_MAUI_Samples.Views
{
    public partial class QRScannerPage : ContentPage
    {
        public QRScannerPage(QRScannerViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
