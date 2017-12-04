using Cringebot.ViewModel;
using NUnit.Framework;

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
        
        public class ToggleSimulateCommandProperty
        {
            [Test]
            public void ShouldToggleSimulate()
            {
                //arrange

                //act

                //assert
            }
        }
    }
}
