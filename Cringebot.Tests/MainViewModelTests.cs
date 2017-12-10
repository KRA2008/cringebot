using Cringebot.ViewModel;
using NUnit.Framework;
using SharpTestsEx;
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
            public void ShouldInitializeMemories()
            {
                //assert
                _viewModel.Memories.Should().Not.Be.Null();
            }
        }

        public class AddMemoryCommand : MainViewModelTests
        {
            [Test]
            public void ShouldAddMemoryInInputToMemories()
            {
                //arrange
                const string TEST_DESCRIPTION = "that time with that thing";
                _viewModel.MemoryInput = TEST_DESCRIPTION;

                //act
                _viewModel.AddMemoryCommand.Execute(null);

                //assert
                _viewModel.Memories.Single(m => m.Description == TEST_DESCRIPTION);
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
    }
}
