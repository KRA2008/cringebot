﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    [TestFixture]
    public class MainViewModelTests
    {
        private MainViewModel _viewModel;
        private Settings _settings;
        private Mock<IAppProperties> _dataStore;
        private Mock<INotificationManager> _notificationManager;
        private Mock<IKeyboardHelper> _keyboardHelper;
        private Mock<IPageModelCoreMethods> _coreMethods;

        [SetUp]
        public void SetupViewModel()
        {
            _settings = new Settings();
            _notificationManager = new Mock<INotificationManager>();
            _dataStore = new Mock<IAppProperties>();
            _dataStore.Setup(d => d.LoadOrDefault(PropertiesWrapper.SETTINGS_STORE_KEY, It.IsAny<Settings>()))
                .Returns(_settings);
            _dataStore.Setup(d => d.LoadOrDefault(PropertiesWrapper.SIMULATE_STORE_KEY, It.IsAny<bool>())).Returns(false);
            _dataStore.Setup(d => d.LoadOrDefault(PropertiesWrapper.LIMIT_LIST_STORE_KEY, It.IsAny<bool>())).Returns(false);
            _dataStore.Setup(d => d.LoadOrDefault(PropertiesWrapper.MEMORY_LIST_STORE_KEY, It.IsAny<List<Memory>>())).Returns(new List<Memory>());
            _keyboardHelper = new Mock<IKeyboardHelper>();
            _coreMethods = new Mock<IPageModelCoreMethods>();
            _viewModel = new MainViewModel(_dataStore.Object, _notificationManager.Object, _keyboardHelper.Object)
            {
                CoreMethods = _coreMethods.Object
            };
            _viewModel.Init(null);
        }

        public class InitMethod : MainViewModelTests
        {
            [Test, Theory]
            public void ShouldLoadSavedStateOfSimulateSettingOrDefault(bool expectedStoredSetting)
            {
                //arrange
                _dataStore.Setup(w => w.LoadOrDefault(PropertiesWrapper.SIMULATE_STORE_KEY, false)).Returns(expectedStoredSetting);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.Simulate.Should().Be.EqualTo(expectedStoredSetting);
            }

            [Test, Theory]
            public void ShouldLoadSavedStateOfShowListSettingOrDefault(bool expectedStoredSetting)
            {
                //arrange
                _dataStore.Setup(w => w.LoadOrDefault(PropertiesWrapper.LIMIT_LIST_STORE_KEY, false)).Returns(expectedStoredSetting);

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
                _dataStore.Setup(w => w.LoadOrDefault(PropertiesWrapper.MEMORY_LIST_STORE_KEY, new List<Memory>())).Returns(expectedList);

                //act
                _viewModel.Init(null);

                //assert
                _viewModel.Memories.Should().Have.SameSequenceAs(new[] { targetMemory1, targetMemory2 });
            }

            [Test]
            public void ShouldUpdateNotificationManagerIfSimulateOff()
            {
                //arrange
                const string MEM1 = "blah";
                const string MEM2 = "gragh";

                _viewModel.MemoryInput = MEM1;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = MEM2;
                _viewModel.AddMemoryCommand.Execute(null);

                IEnumerable<Memory> actualList = null;
                _notificationManager.Setup(m => m.SetMemories(It.IsAny<IEnumerable<Memory>>())).Callback<IEnumerable<Memory>>(list =>
                {
                    actualList = list;
                });

                //act
                _viewModel.Init(null);

                //assert
                _notificationManager.Verify(n => n.SetSettings(_settings));
                var memoryArray = actualList as Memory[] ?? actualList.ToArray();
                memoryArray.SingleOrDefault(m => m.Description == MEM1).Should().Not.Be.Null();
                memoryArray.SingleOrDefault(m => m.Description == MEM2).Should().Not.Be.Null();
            }

            [Theory]
            public void ShouldStartNotificationsIfSimulateOn()
            {
                //arrange
                const string MEM1 = "blah";
                const string MEM2 = "gragh";

                _viewModel.MemoryInput = MEM1;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = MEM2;
                _viewModel.AddMemoryCommand.Execute(null);

                _dataStore.Setup(d => d.LoadOrDefault(PropertiesWrapper.SIMULATE_STORE_KEY, It.IsAny<bool>()))
                    .Returns(true);

                IEnumerable<Memory> actualList = null;
                _notificationManager.Setup(m => m.StartNotifications(It.IsAny<IEnumerable<Memory>>(), _settings)).Callback<IEnumerable<Memory>,Settings>((list, settings) =>
                {
                    actualList = list;
                });

                //act
                _viewModel.Init(null);

                //assert
                var memoryArray = actualList as Memory[] ?? actualList.ToArray();
                memoryArray.SingleOrDefault(m => m.Description == MEM1).Should().Not.Be.Null();
                memoryArray.SingleOrDefault(m => m.Description == MEM2).Should().Not.Be.Null();
            }
        }

        public class LimitListVisibilityProperty : MainViewModelTests
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
                _viewModel.Memories.Count.Should().Be.EqualTo(0);
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
                _viewModel.Memories.Count.Should().Be.EqualTo(1);
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
                _viewModel.Memories.Count.Should().Be.EqualTo(0);
            }
        }

        public class ReverseInitMethod : MainViewModelTests
        {
            [Test]
            public void ShouldSetMemoriesToPassedMemories()
            {
                //arrange
                var memories = new List<Memory>
                {
                    new Memory()
                };

                //act
                _viewModel.ReverseInit(new SettingsPushPackage
                {
                    Memories = memories
                });

                //assert
                _viewModel.Memories.SequenceEqual(memories).Should().Be.True();
            }

            [Test]
            public void ShouldRemovePassedMemoryFromListIfPassedMemory()
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
                _viewModel.Memories.Count.Should().Be.EqualTo(2);
            }

            [Theory]
            public void ShouldUpdateNotificationMemoryListIfPassedMemory(bool limitListVisibility)
            {
                //arrange
                const string MEM1 = "blah";
                const string MEM2 = "gragh";
                _viewModel.LimitListVisibility = limitListVisibility;

                _viewModel.MemoryInput = MEM1;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = MEM2;
                _viewModel.AddMemoryCommand.Execute(null);

                IEnumerable<Memory> actualList = null;
                _notificationManager.Setup(m => m.SetMemories(It.IsAny<IEnumerable<Memory>>())).Callback<IEnumerable<Memory>>((list) =>
                {
                    actualList = list;
                });

                //act
                _viewModel.ReverseInit(new Memory());

                //assert
                var memoryArray = actualList as Memory[] ?? actualList.ToArray();
                memoryArray.SingleOrDefault(m => m.Description == MEM1).Should().Not.Be.Null();
                memoryArray.SingleOrDefault(m => m.Description == MEM2).Should().Not.Be.Null();
            }

            public class PropertyChangedTests : ReverseInitMethod
            {
                [TestCase("Memories", true)]
                [TestCase("Memories", false)]
                [TestCase("SearchResultCount", true)]
                [TestCase("SearchResultCount", false)]
                public void ShouldRaisePropertyChangedOnMemories(string propertyName, bool isMemory)
                {
                    //arrange
                    var eventRaised = false;
                    _viewModel.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == propertyName)
                        {
                            eventRaised = true;
                        }
                    };

                    //act
                    if (isMemory)
                    {
                        ReverseInitMemory();
                    }
                    else
                    {
                        ReverseInitPackage();
                    }

                    //assert
                    eventRaised.Should().Be.True();
                }

                private void ReverseInitMemory()
                {
                    _viewModel.ReverseInit(new Memory());
                }

                private void ReverseInitPackage()
                {
                    _viewModel.ReverseInit(new SettingsPushPackage());
                }
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

                var fakeNow = new DateTime(1234, 5, 6, 7, 8, 9, 10);
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
                _viewModel.Memories.Count.Should().Be.EqualTo(1);
            }

            [Theory]
            public void ShouldUpdateNotificationMemoryList(bool limitListVisibility)
            {
                //arrange
                const string MEM1 = "blah";
                const string MEM2 = "gragh";
                _viewModel.LimitListVisibility = limitListVisibility;

                IEnumerable<Memory> actualList = null;
                _notificationManager.Setup(m => m.SetMemories(It.IsAny<IEnumerable<Memory>>())).Callback<IEnumerable<Memory>>((list) => 
                {
                    actualList = list;
                });

                //act
                _viewModel.MemoryInput = MEM1;
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = MEM2;
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                var memoryArray = actualList as Memory[] ?? actualList.ToArray();
                memoryArray.SingleOrDefault(m => m.Description == MEM1).Should().Not.Be.Null();
                memoryArray.SingleOrDefault(m => m.Description == MEM2).Should().Not.Be.Null();
            }

            [Test]
            public void ShouldRaisePropertyChangedOnMemories()
            {
                //arrange
                var eventRaised = false;
                _viewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(_viewModel.Memories))
                    {
                        eventRaised = true;
                    }
                };

                //act
                _viewModel.MemoryInput = "whatever";
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                eventRaised.Should().Be.True();
            }

            [Test, Theory]
            public void Should_trigger_cringe_flash(bool currentTriggerState)
            {
                //arrange
                _viewModel.CringeFlashTrigger = currentTriggerState;

                //act
                _viewModel.MemoryInput = "hi";
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                _viewModel.CringeFlashTrigger.Should().Be.EqualTo(!currentTriggerState);
            }
        }

        public class AddOccurrenceCommand : MainViewModelTests
        {
            [Test]
            public void ShouldAddOccurrenceOfMemoryAtStartOfList()
            {
                //arrange
                var memory = new Memory();
                var now = new DateTime(1, 2, 3, 4, 5, 6, 7);
                SystemTime.Now = () => now;

                //act
                _viewModel.AddOccurrenceCommand.Execute(memory);
                SystemTime.Now = () => now.AddSeconds(1);
                _viewModel.AddOccurrenceCommand.Execute(memory);
                SystemTime.Now = () => now.AddSeconds(2);
                _viewModel.AddOccurrenceCommand.Execute(memory);

                //assert
                memory.Occurrences.ElementAt(0).Should().Be.EqualTo(now.AddSeconds(2));
                memory.Occurrences.ElementAt(1).Should().Be.EqualTo(now.AddSeconds(1));
                memory.Occurrences.ElementAt(2).Should().Be.EqualTo(now);
            }

            [Test]
            public void ShouldClearInput()
            {
                //arrange
                _viewModel.MemoryInput = "blahblah";

                //act
                _viewModel.AddOccurrenceCommand.Execute(new Memory());

                //assert
                _viewModel.MemoryInput.Should().Be.Empty();
            }

            [Test]
            public void ShouldHideKeyboard()
            {
                //act
                _viewModel.AddOccurrenceCommand.Execute(new Memory());

                //assert
                _keyboardHelper.Verify(h => h.HideKeyboard());
            }

            [Test, Theory]
            public void Should_trigger_cringe_flash(bool currentTriggerState)
            {
                //arrange
                _viewModel.CringeFlashTrigger = currentTriggerState;

                //act
                _viewModel.AddOccurrenceCommand.Execute(new Memory());

                //assert
                _viewModel.CringeFlashTrigger.Should().Be.EqualTo(!currentTriggerState);
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
                coreMethods.Verify(c => c.PushPageModel<DetailsViewModel>(memory, false, true));
            }
        }

        public class SaveMethod : MainViewModelTests
        {
            [Theory]
            public void ShouldSaveStateOfSimulate(bool simulate)
            {
                //arrange
                _viewModel.Simulate = simulate;

                //act
                _viewModel.Save();

                //assert
                _dataStore.Verify(d => d.Save(PropertiesWrapper.SIMULATE_STORE_KEY, simulate));
            }

            [Theory]
            public void ShouldSaveStateOfShowList(bool showList)
            {
                //arrange
                _viewModel.LimitListVisibility = showList;

                //act
                _viewModel.Save();

                //assert
                _dataStore.Verify(d => d.Save(PropertiesWrapper.LIMIT_LIST_STORE_KEY, showList));
            }

            [Test]
            public void ShouldSaveSettings()
            {
                //arrange
                var settings = new Settings();
                _dataStore.Setup(d => d.LoadOrDefault(PropertiesWrapper.SETTINGS_STORE_KEY, It.IsAny<Settings>()))
                    .Returns(settings);

                _viewModel.Init(null);

                //act
                _viewModel.Save();

                //assert
                _dataStore.Verify(d => d.Save(PropertiesWrapper.SETTINGS_STORE_KEY, settings));
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

                // ReSharper disable once UnusedVariable
                var whatever = _viewModel.Memories; // trigger filtering

                IEnumerable<Memory> actualSavedList = null;
                _dataStore.Setup(d => d.Save(PropertiesWrapper.MEMORY_LIST_STORE_KEY, It.IsAny<object>())).Callback<string, object>((s, l) =>
                {
                    actualSavedList = (IEnumerable<Memory>)l;
                });

                //act
                _viewModel.Save();

                //assert
                var memoryArray = actualSavedList as Memory[] ?? actualSavedList.ToArray();
                memoryArray.Single(m => m.Description == IN_MEMORY_DESCRIPTION).Should().Not.Be.Null();
                memoryArray.Single(m => m.Description == OUT_MEMORY_DESCRIPTION).Should().Not.Be.Null();
                memoryArray.Length.Should().Be.EqualTo(2);
            }
        }

        public class SimulateProperty : MainViewModelTests
        {
            [Test]
            public void ShouldTurnOnNotificationsWhenSwitchedOn()
            {
                //arrange
                const string IN_MEMORY_DESCRIPTION = "a";

                _viewModel.MemoryInput = IN_MEMORY_DESCRIPTION;
                _viewModel.AddMemoryCommand.Execute(null);

                IEnumerable<Memory> actualSavedList = null;
                _notificationManager.Setup(m => m.StartNotifications(It.IsAny<IEnumerable<Memory>>(), _settings)).Callback<IEnumerable<Memory>,Settings>((list, settings) =>
                {
                    actualSavedList = list;
                });

                //act
                _viewModel.Simulate = true;

                //assert
                var memoryArray = actualSavedList as Memory[] ?? actualSavedList.ToArray();
                memoryArray.Single(m => m.Description == IN_MEMORY_DESCRIPTION).Should().Not.Be.Null();
            }

            [Test]
            public void ShouldTurnOffNotificationsWhenSwitchedOff()
            {
                //arrange
                _viewModel.Simulate = true;

                //act
                _viewModel.Simulate = false;

                //assert
                _notificationManager.Verify(m => m.StopNotifications());
            }
        }

        public class ViewGraphMethod : MainViewModelTests
        {
            [Test]
            public async Task ShouldNavigateToStatsPage()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;
                var memories = new List<Memory>();

                //act
                await _viewModel.ViewGraph(memories);

                //assert
                coreMethods.Verify(c => c.PushPageModel<ChartViewModel>(memories, false, true));
            }
        }

        public class ViewHelpMethod : MainViewModelTests
        {
            [Test]
            public async Task ShouldNavigateToHelpPage()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;

                //act
                await _viewModel.ViewHelp();

                //assert
                coreMethods.Verify(c => c.PushPageModel<HelpViewModel>(true));
            }
        }

        public class ViewSettingsMethod : MainViewModelTests
        {
            [Test]
            public async Task ShouldNavigateToSettingsPage()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;

                _viewModel.Init(null); // load saved settings

                //act
                await _viewModel.ViewSettings();

                //assert
                coreMethods.Verify(c =>
                    c.PushPageModel<SettingsViewModel>(It.Is<SettingsPushPackage>(s => 
                            s.Settings == _settings &&
                            s.Memories.SequenceEqual(_viewModel.Memories)
                            ), false,
                        true));
            }
        }

        public class ImportMethod : MainViewModelTests
        {
            [Test]
            public void ShouldImportAndDecodeMemories()
            {
                //arrange
                const string INPUT_STRING = "thing1%0Athing2%0Ayet%20another%20thing";


                //act
                _viewModel.Import(INPUT_STRING);

                //assert
                _viewModel.Memories.ElementAt(0).Description.Should().Be.EqualTo("thing1");
                _viewModel.Memories.ElementAt(1).Description.Should().Be.EqualTo("thing2");
                _viewModel.Memories.ElementAt(2).Description.Should().Be.EqualTo("yet another thing");
            }

            [Test]
            public void ShouldRaiseChangedEvent()
            {
                //arrange
                var eventRaised = false;
                _viewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(_viewModel.Memories))
                    {
                        eventRaised = true;
                    }
                };

                //act
                _viewModel.Import("blah blah blah");

                //assert
                eventRaised.Should().Be.True();
            }

            [Test]
            public void ShouldSetNotificationMemoryList()
            {
                //arrange
                const string EXPECTED_MEMORY = "this one";

                //act
                _viewModel.Import(EXPECTED_MEMORY);

                //assert
                _notificationManager.Verify(n =>
                    n.SetMemories(It.Is<IEnumerable<Memory>>(m => m.First().Description == EXPECTED_MEMORY)));
            }
        }

        public class SearchResultCountProperty : MainViewModelTests
        {
            [Test]
            public void ShouldGetSearchResultCountFromMemoryInput()
            {
                //arrange
                _viewModel.MemoryInput = "blah";
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = "blah boo";
                _viewModel.AddMemoryCommand.Execute(null);
                _viewModel.MemoryInput = "blah boo bah";
                _viewModel.AddMemoryCommand.Execute(null);

                //act & assert
                _viewModel.MemoryInput = "blah";
                _viewModel.SearchResultCount.Should().Be.EqualTo(3);
                _viewModel.MemoryInput = "boo";
                _viewModel.SearchResultCount.Should().Be.EqualTo(2);
                _viewModel.MemoryInput = "bah";
                _viewModel.SearchResultCount.Should().Be.EqualTo(1);
            }
        }

        public class ViewIsAppearingMethod : MainViewModelTests
        {
            [Test]
            public async Task Should_go_to_help_page_if_first_opening_ever()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;
                _dataStore.Setup(d => d.LoadOrDefault(PropertiesWrapper.HAS_OPENED_BEFORE, false)).Returns(false);

                //act
                await _viewModel.ViewIsAppearing();

                //assert
                coreMethods.Verify(c => c.PushPageModel<HelpViewModel>(true, false, true));
                _dataStore.Verify(d => d.Save(PropertiesWrapper.HAS_OPENED_BEFORE, true));
            }

            [Test]
            public async Task Should_NOT_go_to_help_page_if_NOT_first_opening_ever()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;
                _dataStore.Setup(d => d.LoadOrDefault(PropertiesWrapper.HAS_OPENED_BEFORE, false)).Returns(true);

                //act
                await _viewModel.ViewIsAppearing();

                //assert
                coreMethods.Verify(c => c.PushPageModel<HelpViewModel>(true, false, true), Times.Never);
            }
        }
    }
}
