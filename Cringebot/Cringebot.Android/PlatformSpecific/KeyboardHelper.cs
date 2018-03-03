using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using Cringebot.Droid.PlatformSpecific;
using Cringebot.Wrappers;
using Xamarin.Forms;

[assembly: Dependency(typeof(KeyboardHelper))]
namespace Cringebot.Droid.PlatformSpecific
{
    public class KeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            var context = Forms.Context;
            if (context.GetSystemService(Context.InputMethodService) is InputMethodManager inputMethodManager && context is Activity)
            {
                var activity = (Activity) context;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }
    }
}