using Cringebot.Model;
using Cringebot.PageModel;
using Cringebot.Utilities;
using Cringebot.Wrappers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using System;
using System.Collections.Generic;

namespace Cringebot.Tests.PageModel
{
    public class DetailsPageModelTests
    {
        protected DetailsPageModel _pageModel;
        private Mock<IAppDataStore> _dataStore;
        private Mock<IInStorageMemoryUpdater> _updater;

        [SetUp]
        public void DoSomething()
        {
            _dataStore = new Mock<IAppDataStore>();
            _dataStore.Setup(d => d.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, It.IsAny<List<Memory>>()))
                .Returns(new List<Memory> { new Memory() });
            _updater = new Mock<IInStorageMemoryUpdater>();
            var starterMemory = new Memory();
            starterMemory.Occurrences.Add(new DateTime());
            _pageModel = new DetailsPageModel(_dataStore.Object, _updater.Object)
            {
                Memory = starterMemory
            };
        }

        public class DeleteOccurrenceCommand : DetailsPageModelTests
        {
            [Test]
            public void ShouldDeleteOccurrence()
            {
                //arrange
                var targetTime = new DateTime(111, 11, 1, 1, 1, 1);

                _pageModel.Memory.Occurrences.Add(targetTime);
                _pageModel.Memory.Occurrences.Contains(targetTime).Should().Be.True();

                //act
                _pageModel.DeleteOccurrenceCommand.Execute(targetTime);

                //assert
                _pageModel.Memory.Occurrences.Contains(targetTime).Should().Be.False();
            }
        }

        public class MemoryProperty : DetailsPageModelTests
        {
            [Test]
            public void ShouldSaveMemoryOnDescriptionChange()
            {
                //act
                _pageModel.Memory.Description = "blahblah";

                //assert
                _updater.Verify(u => u.UpdateMemory(_pageModel.Memory));
            }

            [Test]
            public void ShouldSaveMemoryOnOccurrenceChange()
            {
                //act
                _pageModel.Memory.Occurrences.RemoveAt(0);

                //assert
                _updater.Verify(u => u.UpdateMemory(_pageModel.Memory));
            }
        }
    }
}
