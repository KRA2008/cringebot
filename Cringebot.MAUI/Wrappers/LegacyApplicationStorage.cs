#if __ANDROID__
using Cringebot.Droid.PlatformSpecific;
#elif __IOS__
using Cringebot.iOS.PlatformSpecific;
#endif

namespace Cringebot.Wrappers;

public class LegacyApplication
{
    readonly PropertiesDeserializer deserializer;
    Task<IDictionary<string, object>>? propertiesTask;

    static LegacyApplication? current;
    public static LegacyApplication? Current
    {
        get
        {
            current ??= (LegacyApplication)Activator.CreateInstance(typeof(LegacyApplication));
            return current;
        }
    }

    public LegacyApplication()
    {
        deserializer = new PropertiesDeserializer();
    }

    public IDictionary<string, object> Properties
    {
        get
        {
            propertiesTask ??= GetPropertiesAsync();
            return propertiesTask.Result;
        }
    }

    async Task<IDictionary<string, object>> GetPropertiesAsync()
    {
        IDictionary<string, object> properties = await deserializer.DeserializePropertiesAsync().ConfigureAwait(false);
        properties ??= new Dictionary<string, object>(4);
        return properties;
    }
}