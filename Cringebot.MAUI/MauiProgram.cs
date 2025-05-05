#if __ANDROID__
using Cringebot.Droid.PlatformSpecific;
#elif __IOS__
using Cringebot.iOS;
using Cringebot.iOS.PlatformSpecific;
#endif
using Cringebot.Services;
using Cringebot.Wrappers;
using Microsoft.Extensions.Logging;

namespace Cringebot
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

            builder.ConfigureMauiHandlers(handlers =>
            {
#if __IOS__
#elif __ANDROID__
#endif
            });

            var services = builder.Services;
            services.Add(ServiceDescriptor.Singleton<IPersistentStorage, PersistentStorage>());
            services.Add(ServiceDescriptor.Transient<ICsvParserService, CsvParserService>());
            services.Add(ServiceDescriptor.Singleton<IDeviceWrapper, DeviceWrapper>());
            services.Add(ServiceDescriptor.Singleton<IThemeService, ThemeService>());
#if __IOS__
            services.Add(ServiceDescriptor.Singleton<INotificationManager, AppDelegate>());
            services.Add(ServiceDescriptor.Singleton<IKeyboardHelper, KeyboardHelper>());
#elif __ANDROID__
            services.Add(ServiceDescriptor.Singleton<INotificationManager, MyNotificationManager>());
            services.Add(ServiceDescriptor.Singleton<IKeyboardHelper, KeyboardHelper>());
#endif
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
