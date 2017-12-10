using Newtonsoft.Json;
using Xamarin.Forms;

namespace Cringebot.Wrappers
{
    public interface IAppDataStore
    {
        bool TryLoad<T>(string key, out T data);
        void Save(string key, object data);
    }
    
    // unit test not possible - wrapper to enable unit testing of things for which storage is a dependency
    public class StorageWrapper : IAppDataStore
    {
        public bool TryLoad<T>(string key, out T data)
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                data = JsonConvert.DeserializeObject<T>(Application.Current.Properties[key] as string);
                return true;
            }
            data = default(T);
            return false;
        }

        public void Save(string key, object data)
        {
            Application.Current.Properties[key] = JsonConvert.SerializeObject(data);
        }
    }
}
