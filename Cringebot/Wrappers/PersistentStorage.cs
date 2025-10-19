using CommunityToolkit.Mvvm.Messaging;
using Cringebot.Model;
using Newtonsoft.Json;

namespace Cringebot.Wrappers
{
    public interface IPersistentStorage
    {
        T LoadOrDefault<T>(string key, T defaultValue);
    }
    
    public class PersistentStorage : IPersistentStorage
    {
        public const string MEMORY_LIST_STORE_KEY = "memoryList";
        public const string SETTINGS_STORE_KEY = "settings";
        public const string THEME_STORE_KEY = "theme";

        public PersistentStorage()
        {
            Migrate();
            WeakReferenceMessenger.Default.Register<MemoriesChangedMessage>(this, (recipient, message) =>
            {
                Save(MEMORY_LIST_STORE_KEY, message.Value);
            });
            WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, (recipient, message) =>
            {
                Save(SETTINGS_STORE_KEY, message.Value);
            });
            WeakReferenceMessenger.Default.Register<ThemeChangedMessage>(this, (recipient, message) =>
            {
                Save(THEME_STORE_KEY, message.Value);
            });
        }

        private void Migrate()
        {
            const string SIMULATE_STORE_KEY_OLD = "simulate";
            const string LIMIT_LIST_STORE_KEY_OLD = "limitList";
            const string MEMORY_LIST_STORE_KEY_OLD = "memoryList";
            const string SETTINGS_STORE_KEY_OLD = "settings";
            const string THEME_STORE_KEY_OLD = "theme";
            const string HAS_OPENED_BEFORE_OLD = "openedBefore";

            var existingSettings = new Settings();

            if (LegacyApplication.Current?.Properties.TryGetValue(MEMORY_LIST_STORE_KEY_OLD, out var memory) == true)
            {
                Preferences.Set(MEMORY_LIST_STORE_KEY, (string?)memory);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(SETTINGS_STORE_KEY_OLD, out var settings) == true)
            {
                existingSettings = (Settings) settings;
                Preferences.Set(SETTINGS_STORE_KEY, (string?)settings);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(THEME_STORE_KEY_OLD, out var theme) == true)
            {
                Preferences.Set(THEME_STORE_KEY, (string?)theme);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(HAS_OPENED_BEFORE_OLD, out var opened) == true)
            {
                existingSettings.HasBeenOpenedBefore = bool.Parse((string)opened);
                Save(SETTINGS_STORE_KEY, existingSettings);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(SIMULATE_STORE_KEY_OLD, out var simulate) == true)
            {
                existingSettings.Simulate = bool.Parse((string)simulate);
                Save(SETTINGS_STORE_KEY, existingSettings);
            }
            if (LegacyApplication.Current?.Properties.TryGetValue(LIMIT_LIST_STORE_KEY_OLD, out var limit) == true)
            {
                existingSettings.LimitListVisibility = bool.Parse((string)limit);
                Save(SETTINGS_STORE_KEY, existingSettings);
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

        private void Save(string key, object data)
        {
            Preferences.Set(key, JsonConvert.SerializeObject(data));
        }
    }
}
