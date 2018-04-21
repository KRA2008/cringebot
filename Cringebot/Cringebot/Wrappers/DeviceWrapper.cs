using System;
using Xamarin.Forms;

namespace Cringebot.Wrappers
{
    public interface IDeviceWrapper
    {
        void OpenUri(string uri);
        string RuntimePlatform();
    }

    public class DeviceWrapper : IDeviceWrapper
    {
        public void OpenUri(string uri)
        {
            Device.OpenUri(new Uri(uri));
        }

        public string RuntimePlatform()
        {
            return Device.RuntimePlatform;
        }
    }
}