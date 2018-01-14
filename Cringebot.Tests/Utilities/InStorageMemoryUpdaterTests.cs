using Cringebot.Model;
using Cringebot.Utilities;
using Cringebot.Wrappers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Cringebot.Tests.Utilities
{
    public class InStorageMemoryUpdaterTests
    {
        private InStorageMemoryUpdater _updater;
        private Mock<IAppDataStore> _dataStore;

        [SetUp]
        public void Instantiate()
        {
            _dataStore = new Mock<IAppDataStore>();
            _updater = new InStorageMemoryUpdater(_dataStore.Object);
        }

        public class UpdateMemoryMethod : InStorageMemoryUpdaterTests
        {
            [Test]
            public void ShouldReplaceAndSort()
            {
                //arrange
                const string TARGET_MEMORY_ID = "blah";
                var targetMemory = new Memory
                {
                    ID = TARGET_MEMORY_ID,
                    Description = "1"
                };
                var replacementMemory = new Memory
                {
                    ID = TARGET_MEMORY_ID,
                    Description = "b"
                };
                var bystanderMemory1 = new Memory
                {
                    ID = "whatever",
                    Description = "a"
                };
                var bystanderMemory2 = new Memory
                {
                    ID = "Teehee",
                    Description = "c"
                };

                var fakeList = new List<Memory>
                {
                    targetMemory,
                    bystanderMemory1,
                    bystanderMemory2
                };

                _dataStore.Setup(d => d.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, It.IsAny<List<Memory>>())).Returns(fakeList);

                //act
                _updater.UpdateMemory(replacementMemory);

                //assert
                _dataStore.Verify(d => d.Save(StorageWrapper.MEMORY_LIST_STORE_KEY, It.Is<List<Memory>>(
                    l => l.SequenceEqual(new[] { bystanderMemory1, replacementMemory, bystanderMemory2 }))));
            }
        }
    }
}
