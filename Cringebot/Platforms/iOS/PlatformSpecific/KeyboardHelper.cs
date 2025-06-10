using Cringebot.Wrappers;
using UIKit;

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