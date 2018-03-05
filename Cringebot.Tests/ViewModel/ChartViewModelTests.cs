using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cringebot.Model;
using Cringebot.ViewModel;
using FreshMvvm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.ViewModel
{
    public class ChartViewModelTests
    {
        private ChartViewModel _viewModel;
        private Mock<IPageModelCoreMethods> _coreMethods;

        [SetUp]
        public void Instantiate()
        {
            _coreMethods = new Mock<IPageModelCoreMethods>();
            _viewModel = new ChartViewModel
            {
                CoreMethods = _coreMethods.Object
            };
        }

        public class InitMethod : ChartViewModelTests
        {
            [Test]
            public void ShouldAddDescriptionSuffixToTitleWhenSingleMemory()
            {
                //arrange
                const string EXPECTED_SUFFIX = "blah blah";
                var memories = new List<Memory>{new Memory
                {
                    Description = EXPECTED_SUFFIX
                }};

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Title.Should().Be.EqualTo("Chart (" + EXPECTED_SUFFIX + ")");
            }

            [Test]
            public void ShouldJustSetTitleWhenMultipleMemories()
            {
                //arrange
                var memories = new List<Memory>{new Memory
                    {
                        Description = "whatever"
                    },
                    new Memory
                    {
                        Description = "oh boy"
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Title.Should().Be.EqualTo("Chart");
            }

            [Test]
            public void ShouldSetNoDataIfNoMemories()
            {
                //act
                _viewModel.Init(new List<Memory>());

                //assert
                _viewModel.IsDataPresent.Should().Be.False();
            }

            [Test]
            public void ShouldSetNoDataIfNoOccurrences()
            {
                //act
                _viewModel.Init(new List<Memory>
                {
                    new Memory()
                });

                //assert
                _viewModel.IsDataPresent.Should().Be.False();
            }

            [Test]
            public void ShouldSetYesDataIfData()
            {
                //act
                _viewModel.Init(new List<Memory>
                {
                    new Memory
                    {
                        Occurrences = { new DateTime()}
                    }
                });

                //assert
                _viewModel.IsDataPresent.Should().Be.True();
            }
        }

        public class NavigateToStatsMethod : ChartViewModelTests
        {
            [Test]
            public async Task ShouldNavigateToStats()
            {
                //arrange
                var memories = new List<Memory>{new Memory
                    {
                        Description = "whatever"
                    },
                    new Memory
                    {
                        Description = "oh boy"
                    }
                };
                _viewModel.Init(memories);

                //act
                await _viewModel.ViewStats();

                //assert
                _coreMethods.Verify(c => c.PushPageModel<StatsViewModel>(memories, false, true));
            }
        }
    }
}