#if __ANDROID__
using Cringebot.Droid.PlatformSpecific;
#elif __IOS__
using Cringebot.iOS;
using Cringebot.iOS.PlatformSpecific;
#endif
using Cringebot.Page;
using Cringebot.Services;
using Cringebot.ViewModel;
using Cringebot.Wrappers;
using FreshMvvm.Maui.Extensions;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace Cringebot
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = 
                MauiApp.CreateBuilder()
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureMauiHandlers(handlers =>
            {
#if __IOS__
#elif __ANDROID__
#endif
            });

            var services = builder.Services;

            services.AddSingleton<MainViewModel>();
            services.AddTransient<ChartViewModel>();
            services.AddTransient<DetailsViewModel>();
            services.AddTransient<HelpViewModel>();
            services.AddTransient<ImportExportViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<StatsViewModel>();

            services.AddSingleton<MainPage>();
            services.AddTransient<ChartPage>();
            services.AddTransient<DetailsPage>();
            services.AddTransient<HelpPage>();
            services.AddTransient<ImportExportPage>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<StatsPage>();

            services.AddTransient<IPersistentStorage, PersistentStorage>();
            services.AddTransient<ICsvParserService, CsvParserService>();
            services.AddTransient<IDeviceWrapper, DeviceWrapper>();
            services.AddTransient<IThemeService, ThemeService>();
#if __IOS__
            services.AddTransient<INotificationManager, AppDelegate>();
            services.AddTransient<IKeyboardHelper, KeyboardHelper>();
#elif __ANDROID__
            services.AddTransient<INotificationManager, MyNotificationManager>();
            services.AddTransient<IKeyboardHelper, KeyboardHelper>();
#endif
#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();
            app.UseFreshMvvm();
            return app;
        }
    }
}
