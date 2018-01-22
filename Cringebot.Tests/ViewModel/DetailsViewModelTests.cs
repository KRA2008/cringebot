using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cringebot.Model;
using Cringebot.Wrappers;
using Cringebot.ViewModel;
using FreshMvvm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.ViewModel
{
    public class DetailsViewModelTests
    {
        protected DetailsViewModel ViewModel;
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
            ViewModel = new DetailsViewModel
            {
                Memory = starterMemory,
                CoreMethods = _coreMethods.Object
            };
        }

        public class InitMethod : DetailsViewModelTests
        {
            [Test]
            public void ShouldBindPassedMemory()
            {
                //arrange
                var targetMemory = new Memory();

                //act
                ViewModel.Init(targetMemory);

                //assert
                ViewModel.Memory.Should().Be.SameInstanceAs(targetMemory);
            }
        }

        public class DeleteOccurrenceCommand : DetailsViewModelTests
        {
            [Test]
            public void ShouldDeleteOccurrence()
            {
                //arrange
                var targetTime = new DateTime(111, 11, 1, 1, 1, 1);

                ViewModel.Memory.Occurrences.Add(targetTime);
                ViewModel.Memory.Occurrences.Contains(targetTime).Should().Be.True();

                //act
                ViewModel.DeleteOccurrenceCommand.Execute(targetTime);

                //assert
                ViewModel.Memory.Occurrences.Contains(targetTime).Should().Be.False();
            }
        }

        public class DeleteMemoryMethod : DetailsViewModelTests
        {
            [Test]
            public async Task ShouldPopBackAndPassMemory()
            {
                //arrange
                var targetMemory = new Memory();

                //act
                await ViewModel.DeleteMemory(targetMemory);

                //assert
                _coreMethods.Verify(c => c.PopPageModel(targetMemory, false, true));
            }
        }
    }
}
