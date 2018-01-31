using Cringebot.Wrappers;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot.ViewModel
{
    public class HelpViewModel : FreshBasePageModel
    {
        public Command FeedbackCommand { get; }

        public HelpViewModel(IDeviceWrapper deviceWrapper)
        {
            FeedbackCommand = new Command(() =>
            {
                deviceWrapper.OpenUri("mailto:me@kra2008.com?subject=Cringebot%20feedback");
            });
        }
    }
}