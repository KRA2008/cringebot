using Cringebot.Model;
using Cringebot.Wrappers;
using System.Collections.Generic;
using System.Linq;

namespace Cringebot.Utilities
{
    public interface IInStorageMemoryUpdater
    {
        void UpdateMemory(Memory memory);
    }

    public class InStorageMemoryUpdater : IInStorageMemoryUpdater
    {
        private IAppDataStore _dataStore;

        public InStorageMemoryUpdater(IAppDataStore dataStore) 
        {
            _dataStore = dataStore;
        }

        public void UpdateMemory(Memory memory)
        {
            var storedList = _dataStore.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, new List<Memory>());
            storedList.RemoveAll(m => m.ID == memory.ID);
            storedList.Add(memory);
            _dataStore.Save(StorageWrapper.MEMORY_LIST_STORE_KEY, storedList.OrderBy(m => m.Description).ToList());
        }
    }
}
