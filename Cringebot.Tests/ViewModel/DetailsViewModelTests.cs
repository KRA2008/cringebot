using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringebot.Model;
using Cringebot.ViewModel;
using FreshMvvm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.ViewModel
{
    public class DetailsViewModelTests
    {
        private DetailsViewModel _viewModel;
        private Mock<IPageModelCoreMethods> _coreMethods;

        [SetUp]
        public void DoSomething()
        {
            _coreMethods = new Mock<IPageModelCoreMethods>();
            var starterMemory = new Memory();
            starterMemory.Occurrences.Add(new DateTime());
            _viewModel = new DetailsViewModel
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
                _viewModel.Init(targetMemory);

                //assert
                _viewModel.Memory.Should().Be.SameInstanceAs(targetMemory);
            }
        }

        public class ViewStatsMethod : DetailsViewModelTests
        {
            [Test]
            public async Task ShouldNavigateToStats()
            {
                //arrange
                var targetMemory = new Memory();

                //act
                await _viewModel.ViewStats(targetMemory);

                //assert
                _coreMethods.Verify(c =>
                    c.PushPageModel<StatsViewModel>(
                        It.Is<object>(o => ((IEnumerable<Memory>) o).First() == targetMemory), false, true));
            }
        }

        public class DeleteOccurrenceCommand : DetailsViewModelTests
        {
            [Test]
            public void ShouldDeleteOccurrence()
            {
                //arrange
                var targetTime = new DateTime(111, 11, 1, 1, 1, 1);

                _viewModel.Memory.Occurrences.Add(targetTime);
                _viewModel.Memory.Occurrences.Contains(targetTime).Should().Be.True();

                //act
                _viewModel.DeleteOccurrenceCommand.Execute(targetTime);

                //assert
                _viewModel.Memory.Occurrences.Contains(targetTime).Should().Be.False();
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
                await _viewModel.DeleteMemory(targetMemory);

                //assert
                _coreMethods.Verify(c => c.PopPageModel(targetMemory, false, true));
            }
        }
    }
}
