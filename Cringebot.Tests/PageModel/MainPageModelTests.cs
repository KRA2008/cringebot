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
    public class MainPageModelTests
    {
        private MainPageModel _viewModel;
        private Mock<IAppDataStore> _dataStore;
        private Mock<INotificationManager> _notificationManager;

        [SetUp]
        public void SetupViewModel()
        {
            _notificationManager = new Mock<INotificationManager>();
            _dataStore = new Mock<IAppDataStore>();
            _dataStore.Setup(d => d.LoadOrDefault(StorageWrapper.SIMULATE_STORE_KEY, It.IsAny<bool>())).Returns(false);
            _dataStore.Setup(d => d.LoadOrDefault(StorageWrapper.LIMIT_LIST_STORE_KEY, It.IsAny<bool>())).Returns(false);
            _dataStore.Setup(d => d.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, It.IsAny<List<Memory>>())).Returns(new List<Memory>());
            _viewModel = new MainPageModel(_dataStore.Object, _notificationManager.Object);
            _viewModel.Init(null);
        }

        public class InitMethod : MainPageModelTests
        {
            [Test, Theory]
            public void ShouldLoadSavedStateOfSimulateSettingOrDefault(bool expectedStoredSetting)
            {
                //arrange
                bool fakeStoredSetting = expectedStoredSetting;
                _dataStore.Setup(w => w.LoadOrDefault(StorageWrapper.SIMULATE_STORE_KEY, false)).Returns(expectedStoredSetting);

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
                _dataStore.Setup(w => w.LoadOrDefault(StorageWrapper.LIMIT_LIST_STORE_KEY, false)).Returns(expectedStoredSetting);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.LimitListVisibility.Should().Be.EqualTo(expectedStoredSetting);
            }

            [Test]
            public void ShouldLoadSavedStateOfListOrDefault()
            {
                //arrange
                var targetMemory1 = new Memory
                {
                    Description = "whatever"
                };
                var targetMemory2 = new Memory
                {
                    Description = "whatever"
                };
                var expectedList = new List<Memory>
                {
                    targetMemory1,
                    targetMemory2
                };
                _dataStore.Setup(w => w.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, new List<Memory>())).Returns(expectedList);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.Memories.Should().Have.SameSequenceAs(new[] { targetMemory1, targetMemory2 });
            }

            [Theory]
            public void ShouldUpdateNotificationMemoryList(bool limitListVisibility)
            {
                //arrange
                const string mem1 = "blah";
                const string mem2 = "gragh";
                _viewModel.LimitListVisibility = limitListVisibility;

                _viewModel.MemoryInput = mem1;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = mem2;
                _viewModel.AddMemoryCommand.Execute(null);

                IEnumerable<Memory> actualList = null;
                _notificationManager.Setup(m => m.SetMemories(It.IsAny<IEnumerable<Memory>>())).Callback<IEnumerable<Memory>>((list) =>
                {
                    actualList = list;
                });

                //act
                _viewModel.Init(null);

                //assert
                actualList.SingleOrDefault(m => m.Description == mem1).Should().Not.Be.Null();
                actualList.SingleOrDefault(m => m.Description == mem2).Should().Not.Be.Null();
            }
        }

        public class LimitListVisibilityProperty : MainPageModelTests
        {
            [Test]
            public void ShouldCauseMemoryListToShowNothingWhenMoreThanOneMemoryMatches()
            {
                //arrange
                _viewModel.MemoryInput = "aaa1";
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = "aaa2";
                _viewModel.AddMemoryCommand.Execute(null);

                //act
                _viewModel.LimitListVisibility = true;
                _viewModel.MemoryInput = "aaa";

                //assert
                _viewModel.Memories.Count().Should().Be.EqualTo(0);
            }

            [Test]
            public void ShouldCauseMemoryListToShowOneEntryWhenExactlyOneMemoryMatchesAndThreeLettersTyped()
            {
                //arrange
                _viewModel.MemoryInput = "aaa1";
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = "bbb2";
                _viewModel.AddMemoryCommand.Execute(null);

                //act
                _viewModel.LimitListVisibility = true;
                _viewModel.MemoryInput = "aaa";

                //assert
                _viewModel.Memories.Count().Should().Be.EqualTo(1);
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase("a")]
            [TestCase("aa")]
            public void ShouldCauseMemoryListToShowNothingWhenOneMemoryMatchesButFewerThanThreeLettersTyped(string search)
            {
                //arrange
                _viewModel.MemoryInput = "aaa1";
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.AddMemoryCommand.Execute(null);

                //act
                _viewModel.LimitListVisibility = true;
                _viewModel.MemoryInput = search;

                //assert
                _viewModel.Memories.Count().Should().Be.EqualTo(0);
            }
        }

        public class ReverseInitMethod : MainPageModelTests
        {
            [Test]
            public void ShouldRemovePassedMemoryFromList()
            {
                //arrange
                const string TARGET_MEMORY_DESCRIPTION = "something to delete";

                _viewModel.MemoryInput = "blah blah keep me";
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = TARGET_MEMORY_DESCRIPTION;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = "blah blah keep me too";
                _viewModel.AddMemoryCommand.Execute(null);

                var targetMemory = _viewModel.Memories.Single(m => m.Description == TARGET_MEMORY_DESCRIPTION);

                //act
                _viewModel.ReverseInit(targetMemory);

                //assert
                _viewModel.Memories.Should().Not.Contain(targetMemory);
                _viewModel.Memories.Count().Should().Be.EqualTo(2);
            }

            [Theory]
            public void ShouldUpdateNotificationMemoryList(bool limitListVisibility)
            {
                //arrange
                const string mem1 = "blah";
                const string mem2 = "gragh";
                _viewModel.LimitListVisibility = limitListVisibility;

                _viewModel.MemoryInput = mem1;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = mem2;
                _viewModel.AddMemoryCommand.Execute(null);

                IEnumerable<Memory> actualList = null;
                _notificationManager.Setup(m => m.SetMemories(It.IsAny<IEnumerable<Memory>>())).Callback<IEnumerable<Memory>>((list) =>
                {
                    actualList = list;
                });

                //act
                _viewModel.ReverseInit(new Memory());

                //assert
                actualList.SingleOrDefault(m => m.Description == mem1).Should().Not.Be.Null();
                actualList.SingleOrDefault(m => m.Description == mem2).Should().Not.Be.Null();
            }
        }

        public class AddMemoryCommand : MainPageModelTests
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

            [Theory]
            public void ShouldUpdateNotificationMemoryList(bool limitListVisibility)
            {
                //arrange
                const string mem1 = "blah";
                const string mem2 = "gragh";
                _viewModel.LimitListVisibility = limitListVisibility;

                IEnumerable<Memory> actualList = null;
                _notificationManager.Setup(m => m.SetMemories(It.IsAny<IEnumerable<Memory>>())).Callback<IEnumerable<Memory>>((list) => 
                {
                    actualList = list;
                });

                //act
                _viewModel.MemoryInput = mem1;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = mem2;
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                actualList.SingleOrDefault(m => m.Description == mem1).Should().Not.Be.Null();
                actualList.SingleOrDefault(m => m.Description == mem2).Should().Not.Be.Null();
            }
        }

        public class AddOccurrenceCommand : MainPageModelTests
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

        public class MemoryInputProperty : MainPageModelTests
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

        public class DisplayMemoriesProperty : MainPageModelTests
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

        public class ViewDetailsCommand : MainPageModelTests
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

        public class SaveMethod : MainPageModelTests
        {
            [Theory]
            public void ShouldSaveStateOfSimulate(bool simulate)
            {
                //arrange
                _viewModel.Simulate = simulate;

                //act
                _viewModel.Save();

                //assert
                _dataStore.Verify(d => d.Save(StorageWrapper.SIMULATE_STORE_KEY, simulate));
            }

            [Theory]
            public void ShouldSaveStateOfShowList(bool showList)
            {
                //arrange
                _viewModel.LimitListVisibility = showList;

                //act
                _viewModel.Save();

                //assert
                _dataStore.Verify(d => d.Save(StorageWrapper.LIMIT_LIST_STORE_KEY, showList));
            }

            [Test]
            public void ShouldSaveStateOfList()
            {
                //arrange
                const string IN_MEMORY_DESCRIPTION = "a";
                const string OUT_MEMORY_DESCRIPTION = "b";

                _viewModel.MemoryInput = IN_MEMORY_DESCRIPTION;
                _viewModel.AddMemoryCommand.Execute(null);

                _viewModel.MemoryInput = OUT_MEMORY_DESCRIPTION;
                _viewModel.AddMemoryCommand.Execute(null);

                _viewModel.MemoryInput = IN_MEMORY_DESCRIPTION;

                var whatever = _viewModel.Memories; // trigger filtering

                IEnumerable<Memory> actualSavedList = null;
                _dataStore.Setup(d => d.Save(StorageWrapper.MEMORY_LIST_STORE_KEY, It.IsAny<object>())).Callback<string, object>((s, l) =>
                {
                    actualSavedList = (IEnumerable<Memory>)l;
                });

                //act
                _viewModel.Save();

                //assert
                actualSavedList.Single(m => m.Description == IN_MEMORY_DESCRIPTION).Should().Not.Be.Null();
                actualSavedList.Single(m => m.Description == OUT_MEMORY_DESCRIPTION).Should().Not.Be.Null();
                actualSavedList.Count().Should().Be.EqualTo(2);
            }
        }

        public class SimulateProperty : MainPageModelTests
        {
            [Test]
            public void ShouldTurnOnNotificationsWhenSwitchedOn()
            {
                //act
                _viewModel.Simulate = true;

                //assert
                _notificationManager.Verify(m => m.ActivateNotifications());
            }

            [Test]
            public void ShouldTurnOffNotificationsWhenSwitchedOff()
            {
                //arrange
                _viewModel.Simulate = true;

                //act
                _viewModel.Simulate = false;

                //assert
                _notificationManager.Verify(m => m.CancelNotifications());
            }
        }
    }
}
