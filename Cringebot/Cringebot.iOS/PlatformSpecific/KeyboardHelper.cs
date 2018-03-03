using Cringebot.iOS.PlatformSpecific;
using Cringebot.Wrappers;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(KeyboardHelper))]
namespace Cringebot.iOS.PlatformSpecific
{
    public class KeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            UIApplication.SharedApplication.KeyWindow.EndEditing(true);
        }
    }
}