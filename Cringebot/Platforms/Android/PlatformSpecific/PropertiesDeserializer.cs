using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml;

namespace Cringebot.Droid.PlatformSpecific;

public class PropertiesDeserializer
{
    const string PropertyStoreFile = "PropertyStore.forms";

    public Task<IDictionary<string, object>> DeserializePropertiesAsync()
    {
        // Deserialize property dictionary to local storage
        return Task.Run(() =>
        {
            using var store = IsolatedStorageFile.GetUserStoreForApplication();
            if (!store.FileExists(PropertyStoreFile))
                return null;

            using var stream = store.OpenFile(PropertyStoreFile, FileMode.Open, FileAccess.Read);
            using var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
            if (stream.Length == 0)
                return null;

            try
            {
                var dcs = new DataContractSerializer(typeof(Dictionary<string, object>));
                return (IDictionary<string, object>)dcs.ReadObject(reader);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not deserialize properties: " + e.Message);
                Console.WriteLine($"PropertyStore Exception while reading Application properties: {e}");
            }

            return null;
        });
    }
}