using System.Threading.Tasks;
using Cringebot.ViewModel;
using Cringebot.Wrappers;
using FreshMvvm;
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

        public class ViewIsAppearingMethod : HelpViewModelTests
        {
            [Test]
            public async Task Should_pop_thank_you_text_if_passed_true_in_init()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;

                //act
                _viewModel.Init(true);
                await _viewModel.ViewIsAppearing();

                //assert
                coreMethods.Verify(c => c.DisplayAlert("Welcome!", "Thanks for downloading Cringebot. This page will let you know how to use Cringebot. Feel free to send me any feedback or questions using the button at the bottom. Navigate back when you're ready to get started.", "OK"));

                coreMethods.ResetCalls();
                await _viewModel.ViewIsAppearing(); // should not happen a second time
                coreMethods.Verify(c => c.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            }

            [Test]
            public void Should_NOT_pop_thank_you_text_if_passed_null_in_init()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;

                //act
                _viewModel.Init(null);

                //assert
                coreMethods.Verify(c => c.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            }
        }
    }
}