using System;
using Xamarin.Forms;

namespace Cringebot.Wrappers
{
    public interface IDeviceWrapper
    {
        void OpenUri(string uri);
    }

    public class DeviceWrapper : IDeviceWrapper
    {
        public void OpenUri(string uri)
        {
            Device.OpenUri(new Uri(uri));
        }
    }
}