using System;
using System.Threading.Tasks;
using Cringebot.Wrappers;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot.ViewModel
{
    public class HelpViewModel : FreshBasePageModel
    {
        public Command FeedbackCommand { get; }

        private bool _shouldPopWelcomeText;

        public HelpViewModel(IDeviceWrapper deviceWrapper)
        {
            FeedbackCommand = new Command(() =>
            {
                deviceWrapper.OpenUri("mailto:me@kra2008.com?subject=Cringebot%20feedback");
            });
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            _shouldPopWelcomeText = initData is bool b ? b : false;
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            await Task.Delay(100);
            await ViewIsAppearing();
        }

        public async Task ViewIsAppearing()
        {
            if (_shouldPopWelcomeText)
            {
                await CoreMethods.DisplayAlert("Welcome!",
                    "Thanks for downloading Cringebot. This page will let you know how to use Cringebot. Feel free to send me any feedback or questions using the button at the bottom. Navigate back when you're ready to get started.",
                    "OK");
                _shouldPopWelcomeText = false;
            }
        }
    }
}