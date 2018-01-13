using Cringebot.Model;
using Cringebot.ViewModel;
using Cringebot.Wrappers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using System.Collections.Generic;
using System.Linq;

namespace Cringebot.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        private MainViewModel _viewModel;
        private Mock<IAppDataStore> _dataStore;
        private const string SIMULATE_STORE_KEY = "simulate";
        private const string SHOW_LIST_STORE_KEY = "showList";
        private const string MEMORY_LIST_STORE_KEY = "memoryList";

        [SetUp]
        public void InstantiateMainViewModel()
        {
            _dataStore = new Mock<IAppDataStore>();
            _viewModel = new MainViewModel(_dataStore.Object);
            _viewModel.Init(null);
        }

        public class Ctor : MainViewModelTests
        {
            [Test]
            public void ShouldStartWithShowListOn()
            {
                //assert
                _viewModel.ShowList.Should().Be.True();
            }

            [Test]
            public void ShouldInitializeMemories()
            {
                //assert
                _viewModel.DisplayMemories.Should().Not.Be.Null();
            }
        }

        public class InitMethod : MainViewModelTests
        {
            [Test, Theory]
            public void ShouldLoadSavedStateOfSimulateSetting(bool expectedStoredSetting)
            {
                //arrange
                bool fakeStoredSetting = expectedStoredSetting;
                _dataStore.Setup(w => w.TryLoad(SIMULATE_STORE_KEY, out fakeStoredSetting)).Returns(true);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.Simulate.Should().Be.EqualTo(expectedStoredSetting);
            }

            [Test]
            public void ShouldTurnOnSimulateSettingIfNotStored()
            {
                //arrange
                bool dummyObject;
                _dataStore.Setup(w => w.TryLoad(SIMULATE_STORE_KEY, out dummyObject)).Returns(false);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.Simulate.Should().Be.True();
            }

            [Test, Theory]
            public void ShouldLoadSavedStateOfShowListSetting(bool expectedStoredSetting)
            {
                //arrange
                bool fakeStoredSetting = expectedStoredSetting;
                _dataStore.Setup(w => w.TryLoad(SHOW_LIST_STORE_KEY, out fakeStoredSetting)).Returns(true);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.ShowList.Should().Be.EqualTo(expectedStoredSetting);
            }

            [Test]
            public void ShouldTurnOnShowListSettingIfNotStored()
            {
                //arrange
                bool dummyObject;
                _dataStore.Setup(w => w.TryLoad(SHOW_LIST_STORE_KEY, out dummyObject)).Returns(false);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.ShowList.Should().Be.True();
            }

            [Test]
            public void ShouldLoadSavedStateOfList()
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
                _dataStore.Setup(w => w.TryLoad(MEMORY_LIST_STORE_KEY, out expectedList)).Returns(true);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.FullListMemories.Should().Have.SameSequenceAs(expectedList);
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

                //act
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                _viewModel.FullListMemories.Single(m => m.Description == TEST_DESCRIPTION);
            }

            [Test]
            public void ShouldSaveListToStore()
            {
                //arrange
                var MEMORY_1 = "blahblah";
                _viewModel.MemoryInput = MEMORY_1;

                //act
                _viewModel.AddMemoryCommand.Execute(null);

                //arrange
                var MEMORY_2 = "more stuff";
                _viewModel.MemoryInput = MEMORY_2;

                //act
                _viewModel.AddMemoryCommand.Execute(null);
                
                //assert
                _dataStore.Verify(d => d.Save(MEMORY_LIST_STORE_KEY, It.Is<List<Memory>>(l => 
                    l.ElementAt(0).Description == MEMORY_1 &&
                    l.ElementAt(1).Description == MEMORY_2)));
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
                _viewModel.MemoryInput = input;
                _viewModel.FullListMemories.Add(new Memory());

                //act
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                _viewModel.FullListMemories.Count.Should().Be.EqualTo(1);
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

            [Test]
            public void ShouldSaveMemoryList()
            {
                //act
                _viewModel.AddOccurrenceCommand.Execute(new Memory());

                //assert
                _dataStore.Verify(d => d.Save(MEMORY_LIST_STORE_KEY, _viewModel.FullListMemories));
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
                    if(args.PropertyName == nameof(_viewModel.DisplayMemories))
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

        public class ShowListProperty : MainViewModelTests
        {
            [Test, Theory]
            public void ShouldSaveStateToStoreWhenSet(bool expectedShowList)
            {
                //act
                _viewModel.ShowList = expectedShowList;

                //assert
                _dataStore.Verify(d => d.Save(SHOW_LIST_STORE_KEY, expectedShowList));
            }
        }

        public class SimulateProperty : MainViewModelTests
        {
            [Test, Theory]
            public void ShouldSaveStateToStoreWhenSet(bool expectedSimulate)
            {
                //act
                _viewModel.Simulate = expectedSimulate;

                //assert
                _dataStore.Verify(d => d.Save(SIMULATE_STORE_KEY, expectedSimulate));
            }
        }

        public class DisplayMemoriesProperty : MainViewModelTests
        {
            [Test]
            public void ShouldFilterByMemoryInput()
            {
                //arrange
                const string KEYWORD = "LFKJOWOID";
                var mem1 = new Memory
                {
                    Description = "something " + KEYWORD + " what"
                };
                var mem2 = new Memory
                {
                    Description = "lalala"
                };
                var mem3 = new Memory
                {
                    Description = KEYWORD.ToLower()
                };
                _viewModel.FullListMemories = new List<Memory>()
                {
                    mem1,
                    mem2,
                    mem3
                };

                //act
                _viewModel.MemoryInput = KEYWORD;

                //assert
                _viewModel.DisplayMemories.Should().Have.SameSequenceAs(new[] { mem1, mem3 });
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ShouldDisplayFullListWhenNoMemoryInput(string input)
            {
                //arrange
                var mem1 = new Memory
                {
                    Description = "a"
                };
                var mem2 = new Memory
                {
                    Description = "b"
                };
                var mem3 = new Memory
                {
                    Description = "c"
                };
                _viewModel.FullListMemories = new List<Memory> { mem1, mem2, mem3 };

                //act
                _viewModel.MemoryInput = input;

                //assert
                _viewModel.DisplayMemories.Should().Have.SameSequenceAs(new[] { mem1, mem2, mem3 });
            }
        }
    }
}
