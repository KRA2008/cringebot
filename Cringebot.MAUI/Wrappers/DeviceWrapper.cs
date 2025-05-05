namespace Cringebot.Wrappers
{
    public interface IDeviceWrapper
    {
        Task OpenUri(string uri);
        string RuntimePlatform();
    }

    public class DeviceWrapper : IDeviceWrapper
    {
        public async Task OpenUri(string uri)
        {
            await Browser.OpenAsync(uri);
        }

        public string RuntimePlatform()
        {
            return Device.RuntimePlatform;
        }
    }
}