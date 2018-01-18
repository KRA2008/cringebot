using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cringebot.PageModel.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        private MainPageModel _viewModel;
        private Mock<IAppDataStore> _dataStore;

        [SetUp]
        public void SetupViewModel()
        {
            _dataStore = new Mock<IAppDataStore>();
            _dataStore.Setup(d => d.LoadOrDefault(StorageWrapper.SIMULATE_STORE_KEY, It.IsAny<bool>())).Returns(true);
            _dataStore.Setup(d => d.LoadOrDefault(StorageWrapper.SHOW_LIST_STORE_KEY, It.IsAny<bool>())).Returns(true);
            _dataStore.Setup(d => d.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, It.IsAny<List<Memory>>())).Returns(new List<Memory>());
            _viewModel = new MainPageModel(_dataStore.Object);
            _viewModel.Init(null);
        }

        public class InitMethod : MainViewModelTests
        {
            [Test, Theory]
            public void ShouldLoadSavedStateOfSimulateSettingOrDefault(bool expectedStoredSetting)
            {
                //arrange
                bool fakeStoredSetting = expectedStoredSetting;
                _dataStore.Setup(w => w.LoadOrDefault(StorageWrapper.SIMULATE_STORE_KEY, true)).Returns(expectedStoredSetting);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.Simulate.Should().Be.EqualTo(expectedStoredSetting);
            }

            [Test, Theory]
            public void ShouldLoadSavedStateOfShowListSettingOrDefault(bool expectedStoredSetting)
            {
                //arrange
                bool fakeStoredSetting = expectedStoredSetting;
                _dataStore.Setup(w => w.LoadOrDefault(StorageWrapper.SHOW_LIST_STORE_KEY, true)).Returns(expectedStoredSetting);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.ShowList.Should().Be.EqualTo(expectedStoredSetting);
            }

            [Test]
            public void ShouldLoadSavedStateOfListOrDefault()
            {
                //arrange
                var expectedList = new List<Memory>
                {
                    new Memory
                    {
                        Description = "whatever"
                    },
                    new Memory
                    {
                        Description = "more stuff"
                    }
                };
                _dataStore.Setup(w => w.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, new List<Memory>())).Returns(expectedList);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.Memories.Should().Have.SameSequenceAs(expectedList);
            }
        }

        public class AddMemoryCommand : MainViewModelTests
        {
            [Test]
            public void ShouldAddMemoryInInputToFullListMemories()
            {
                //arrange
                const string TEST_DESCRIPTION = "that time with that thing";
                _viewModel.MemoryInput = TEST_DESCRIPTION;

                var fakeNow = new System.DateTime(1234, 5, 6, 7, 8, 9, 10);
                SystemTime.Now = () => fakeNow;

                //act
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                _viewModel.Memories.Single(m => m.Description == TEST_DESCRIPTION).Occurrences.Single().Should().Be.EqualTo(fakeNow);
            }

            [Test]
            public void ShouldClearOutInputField()
            {
                //arrange
                _viewModel.MemoryInput = "some such nonsense";

                //act
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                _viewModel.MemoryInput.Should().Be.Null();
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ShouldNotAddMemoryIfInputIsEmpty(string input)
            {
                //arrange
                _viewModel.MemoryInput = "something whatever!";
                _viewModel.AddMemoryCommand.Execute(null);

                _viewModel.MemoryInput = input;

                //act
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                _viewModel.Memories.Count().Should().Be.EqualTo(1);
            }
        }

        public class AddOccurrenceCommand : MainViewModelTests
        {
            [Test]
            public void ShouldAddOccurrenceOfMemory()
            {
                //arrange
                var memory = new Memory();

                //act
                _viewModel.AddOccurrenceCommand.Execute(memory);
                _viewModel.AddOccurrenceCommand.Execute(memory);
                _viewModel.AddOccurrenceCommand.Execute(memory);

                //assert
                memory.Occurrences.Count.Should().Be.EqualTo(3);
            }
        }

        public class MemoryInputProperty : MainViewModelTests
        {
            [Test]
            public void ShouldRaisePropertyChangedForMemories()
            {
                //arrange
                var eventRaised = false;
                _viewModel.PropertyChanged += (sender, args) =>
                {
                    if(args.PropertyName == nameof(_viewModel.Memories))
                    {
                        eventRaised = true;
                    }
                };

                //act
                _viewModel.MemoryInput = "something!";

                //assert
                eventRaised.Should().Be.True();
            }
        }

        public class DisplayMemoriesProperty : MainViewModelTests
        {
            [Test]
            public void ShouldFilterByMemoryInputAndDisplayAlphabetically()
            {
                //arrange
                const string KEYWORD = "LFKJOWOID";
                var mem1 = "something " + KEYWORD + " what";
                var mem2 = "lalala";
                var mem3 = KEYWORD.ToLower();
                _viewModel.MemoryInput = mem1;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = mem2;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = mem3;
                _viewModel.AddMemoryCommand.Execute(null);

                //act
                _viewModel.MemoryInput = KEYWORD;

                //assert
                _viewModel.Memories.Select(d => d.Description).Should().Have.SameSequenceAs(new[] { mem3, mem1 });
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ShouldDisplayFullListAlphabeticallyWhenNoMemoryInput(string input)
            {
                //arrange
                var mem1 = "b";
                var mem2 = "c";
                var mem3 = "a";
                _viewModel.MemoryInput = mem1;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = mem2;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = mem3;
                _viewModel.AddMemoryCommand.Execute(null);

                //act
                _viewModel.MemoryInput = input;

                //assert
                _viewModel.Memories.Select(d => d.Description).Should().Have.SameSequenceAs(new[] { mem3, mem1, mem2 });
            }
        }

        public class ViewDetailsCommand : MainViewModelTests
        {
            [Test]
            public async Task ShouldNavigateToDetailsPage()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;
                var memory = new Memory();

                //act
                await _viewModel.ViewDetails(memory);

                //assert
                coreMethods.Verify(c => c.PushPageModel<DetailsPageModel>(memory, false, true));
            }
        }
    }
}
