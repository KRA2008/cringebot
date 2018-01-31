using Cringebot.ViewModel;
using Cringebot.Wrappers;
using Moq;
using NUnit.Framework;

namespace Cringebot.Tests.ViewModel
{
    public class HelpViewModelTests
    {
        private HelpViewModel _viewModel;
        private Mock<IDeviceWrapper> _deviceWrapper;

        [SetUp]
        public void InstantiateViewModel()
        {
            _deviceWrapper = new Mock<IDeviceWrapper>();
            _viewModel = new HelpViewModel(_deviceWrapper.Object);
        }

        public class FeedbackCommand : HelpViewModelTests
        {
            [Test]
            public void ShouldOpenEmailUri()
            {
                //act
                _viewModel.FeedbackCommand.Execute(null);

                //assert
                _deviceWrapper.Verify(d => d.OpenUri("mailto:me@kra2008.com?subject=Cringebot%20feedback"));
            }
        }
    }
}