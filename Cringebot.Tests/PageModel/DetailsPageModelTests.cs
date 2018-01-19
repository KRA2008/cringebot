using Cringebot.Model;
using Cringebot.PageModel;
using Cringebot.Wrappers;
using FreshMvvm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cringebot.Tests.PageModel
{
    public class DetailsPageModelTests
    {
        protected DetailsPageModel _pageModel;
        private Mock<IAppDataStore> _dataStore;
        private Mock<IPageModelCoreMethods> _coreMethods;

        [SetUp]
        public void DoSomething()
        {
            _coreMethods = new Mock<IPageModelCoreMethods>();
            _dataStore = new Mock<IAppDataStore>();
            _dataStore.Setup(d => d.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, It.IsAny<List<Memory>>()))
                .Returns(new List<Memory> { new Memory() });
            var starterMemory = new Memory();
            starterMemory.Occurrences.Add(new DateTime());
            _pageModel = new DetailsPageModel(_dataStore.Object)
            {
                Memory = starterMemory
            };
            _pageModel.CoreMethods = _coreMethods.Object;
        }

        public class InitMethod : DetailsPageModelTests
        {
            [Test]
            public void ShouldBindPassedMemory()
            {
                //arrange
                var targetMemory = new Memory();

                //act
                _pageModel.Init(targetMemory);

                //assert
                _pageModel.Memory.Should().Be.SameInstanceAs(targetMemory);
            }
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

        public class DeleteMemoryMethod : DetailsPageModelTests
        {
            [Test]
            public async Task ShouldPopBackAndPassMemory()
            {
                //arrange
                var targetMemory = new Memory();

                //act
                await _pageModel.DeleteMemory(targetMemory);

                //assert
                _coreMethods.Verify(c => c.PopPageModel(targetMemory, false, true));
            }
        }
    }
}
