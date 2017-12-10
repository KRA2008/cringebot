using Cringebot.Model;
using Cringebot.ViewModel;
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

        [SetUp]
        public void InstantiateMainViewModel()
        {
            _viewModel = new MainViewModel();
        }

        public class Ctor : MainViewModelTests
        {
            [Test]
            public void ShouldStartWithSimulateOn()
            {
                //assert
                _viewModel.Simulate.Should().Be.True();
            }

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
            public void ShouldClearOutInputField()
            {
                //arrange
                _viewModel.MemoryInput = "some such nonsense";

                //act
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                _viewModel.MemoryInput.Should().Be.Null();
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
            public void ShouldRaisePropertyChangedForMemories(bool showList)
            {
                //arrange
                _viewModel.ShowList = showList;
                var eventRaised = false;
                _viewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(_viewModel.DisplayMemories))
                    {
                        eventRaised = true;
                    }
                };

                //act
                _viewModel.ShowList = !showList;

                //assert
                eventRaised.Should().Be.True();
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
                    Description = KEYWORD
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

            [Test]
            public void ShouldDisplayNothingIfShowListIsOff()
            {
                //arrange
                _viewModel.FullListMemories = new List<Memory>()
                {
                    new Memory
                    {
                        Description = "whatever"
                    }
                };
                _viewModel.MemoryInput = null;

                //act
                _viewModel.ShowList = false;

                //assert
                _viewModel.DisplayMemories.Should().Be.Empty();
            }
        }
    }
}
