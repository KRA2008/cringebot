﻿using Newtonsoft.Json;
using Xamarin.Forms;

namespace Cringebot.Wrappers
{
    public interface IAppDataStore
    {
        T LoadOrDefault<T>(string key, T defaultValue);
        void Save(string key, object data);
    }
    
    public class StorageWrapper : IAppDataStore
    {
        public const string SIMULATE_STORE_KEY = "simulate";
        public const string SHOW_LIST_STORE_KEY = "showList";
        public const string MEMORY_LIST_STORE_KEY = "memoryList";

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
