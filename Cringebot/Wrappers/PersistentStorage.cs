using Microsoft.Maui.Storage;
using Newtonsoft.Json;

namespace Cringebot.Wrappers
{
    public interface IPersistentStorage
    {
        T LoadOrDefault<T>(string key, T defaultValue);
        void Save(string key, object data);
    }
    
    public class PersistentStorage : IPersistentStorage
    {
        public const string SIMULATE_STORE_KEY = "simulate";
        public const string LIMIT_LIST_STORE_KEY = "limitList";
        public const string MEMORY_LIST_STORE_KEY = "memoryList";
        public const string SETTINGS_STORE_KEY = "settings";
        public const string THEME_STORE_KEY = "theme";
        public const string HAS_OPENED_BEFORE = "openedBefore";

        public PersistentStorage()
        {
            Migrate();
        }

        private void Migrate()
        {
            const string SIMULATE_STORE_KEY_OLD = "simulate";
            const string LIMIT_LIST_STORE_KEY_OLD = "limitList";
            const string MEMORY_LIST_STORE_KEY_OLD = "memoryList";
            const string SETTINGS_STORE_KEY_OLD = "settings";
            const string THEME_STORE_KEY_OLD = "theme";
            const string HAS_OPENED_BEFORE_OLD = "openedBefore";

            if (LegacyApplication.Current?.Properties.TryGetValue(SIMULATE_STORE_KEY_OLD, out var simulate) == true)
            {
                Preferences.Set(SIMULATE_STORE_KEY, (string?)simulate);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(LIMIT_LIST_STORE_KEY_OLD, out var limit) == true)
            {
                Preferences.Set(LIMIT_LIST_STORE_KEY, (string?)limit);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(MEMORY_LIST_STORE_KEY_OLD, out var memory) == true)
            {
                Preferences.Set(MEMORY_LIST_STORE_KEY, (string?)memory);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(SETTINGS_STORE_KEY_OLD, out var settings) == true)
            {
                Preferences.Set(SETTINGS_STORE_KEY, (string?)settings);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(THEME_STORE_KEY_OLD, out var theme) == true)
            {
                Preferences.Set(THEME_STORE_KEY, (string?)theme);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(HAS_OPENED_BEFORE_OLD, out var opened) == true)
            {
                Preferences.Set(HAS_OPENED_BEFORE, (string?)opened);
            }
        }

        public T LoadOrDefault<T>(string key, T defaultValue)
        {
            if (Preferences.ContainsKey(key) &&
                Preferences.Get(key, null) is { } pref)
            {
                return JsonConvert.DeserializeObject<T>(pref);
            }
            return defaultValue;
        }

        public void Save(string key, object data)
        {
            Preferences.Set(key, JsonConvert.SerializeObject(data));
        }
    }
}
