using System;
using System.Globalization;
using Cringebot.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cringebot.Page.ValueConverters
{
    public class DoNotDisturbExplanationValueConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Settings settings)
            {
                return "You will receive no notifications between " + DateTime.Today.Add(settings.DoNotDisturbStartTime).ToString("t") +
                        " and " + DateTime.Today.Add(settings.DoNotDisturbStopTime).ToString("t") + ".";
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