using System;
using System.Globalization;
using Cringebot.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cringebot.Page.ValueConverters
{
    public class GenerationIntervalExplanationValueConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SettingsViewModel settings)
            {
                return "A notification will be generated randomly every " + settings.MinHours +
                       " to " + settings.MaxHours + " hours.";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}