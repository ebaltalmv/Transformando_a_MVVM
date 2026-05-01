using Microsoft.Extensions.Logging;
using MVVM_MAUI_Samples.ViewModels;
using MVVM_MAUI_Samples.Views;
using SharedResources.Models;

namespace MVVM_MAUI_Samples
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Models
            builder.Services.AddSingleton<ConnectivityModel>();
            builder.Services.AddSingleton<GeolocationModel>();
            builder.Services.AddSingleton<MediaPickerModel>();
            builder.Services.AddSingleton<QRScannerModel>();

            // ViewModels
            builder.Services.AddTransient<ConnectivityViewModel>();
            builder.Services.AddTransient<GeolocationViewModel>();
            builder.Services.AddTransient<MediaPickerViewModel>();
            builder.Services.AddTransient<QRScannerViewModel>();

            // Views
            builder.Services.AddTransient<QRScannerPage>();

            return builder.Build();
        }
    }
}
