using Newtonsoft.Json;
using Xamarin.Forms;

namespace Cringebot.Wrappers
{
    public interface IAppDataStore
    {
        T LoadOrDefault<T>(string key, T defaultValue);
        void Save(string key, object data);
    }
    
    // unit test not possible - wrapper to enable unit testing of things for which storage is a dependency
    public class StorageWrapper : IAppDataStore
    {
        public T LoadOrDefault<T>(string key, T defaultValue)
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                return JsonConvert.DeserializeObject<T>(Application.Current.Properties[key] as string);
            }
            else
            {
                return defaultValue;
            }
        }

        public void Save(string key, object data)
        {
            Application.Current.Properties[key] = JsonConvert.SerializeObject(data);
        }
    }
}
