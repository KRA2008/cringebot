using System;
using System.Linq;
using Cringebot.Model;
using Cringebot.ViewModel;
using Cringebot.Wrappers;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.ViewModel
{
    public class StatsViewModelTests
    {
        private StatsViewModel _viewModel;

        [SetUp]
        public void InstantiateViewModel()
        {
            _viewModel = new StatsViewModel();
        }

        public class InitMethod : StatsViewModelTests
        {
            [Test]
            public void ShouldTotalCringes()
            {
                //arrange
                var memories = new[]
                {
                    new Memory
                    {
                        Occurrences = { new DateTime(), new DateTime() }
                    },
                    new Memory
                    {
                        Occurrences = { new DateTime() }
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.First(s => s.Description == "total cringes").Number.Should().Be.EqualTo(3);
            }

            [Test]
            public void ShouldGetTotalDaysTracking()
            {
                //arrange
                var today = new DateTime(444,4,4,4,4,4,4);
                const int DAYS_SINCE = 10;

                SystemTime.Now = () => today;

                var memories = new[]
                {
                    new Memory
                    {
                        Occurrences =
                        {
                            today.AddDays(-1 * DAYS_SINCE),
                            today.AddDays(-4)
                        }
                    },
                    new Memory
                    {
                        Occurrences =
                        {
                            today.AddDays(-1)
                        }
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.First(s => s.Description == "total days using Cringebot").Number.Should().Be
                    .EqualTo(DAYS_SINCE+1);
            }

            [Test]
            public void ShouldGetAverageCringesPerDay()
            {
                //arrange
                var today = new DateTime(44,4,4,4,4,4,4);
                var firstCringe = today.AddDays(-10);

                SystemTime.Now = () => today;

                var memories = new[]
                {
                    new Memory
                    {
                        Occurrences = { today.AddDays(-5) }
                    },
                    new Memory
                    {
                        Occurrences = { firstCringe }
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.First(s => s.Description == "average cringes per day").Number.Should().Be
                    .EqualTo(0.2);
            }

            [Test]
            public void ShouldGetMostCringesInSingleDay()
            {
                //arrange
                var memories = new[]
                {
                    new Memory
                    {
                        Occurrences = {new DateTime(), new DateTime(), new DateTime()}
                    },
                    new Memory
                    {
                        Occurrences = {new DateTime(), new DateTime(), new DateTime(), new DateTime()}
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.First(s => s.Description == "most cringes in single day").Number.Should().Be
                    .EqualTo(4);
            }

            [Test]
            public void ShouldGetTotalDaysWithoutCringes()
            {
                //arrange
                var today = new DateTime(444,4,4,4,4,4,4);
                var firstCringe = today.AddDays(-30);

                SystemTime.Now = () => today;

                var memories = new[]
                {
                    new Memory
                    {
                        Occurrences =
                        {
                            today.AddDays(-5),
                            today.AddDays(-5).AddHours(2)
                        }
                    },
                    new Memory
                    {
                        Occurrences = { firstCringe }
                    },
                    new Memory
                    {
                        Occurrences = { today.AddDays(-10) }
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.First(s => s.Description == "total days without cringe").Number.Should().Be
                    .EqualTo(28);
            }

            [Test]
            public void ShouldGetLongestRunWithoutCringe()
            {
                //arrange
                var today = new DateTime(333, 3, 3, 3, 3, 3, 3);

                SystemTime.Now = () => today;

                var memories = new[]
                {
                    new Memory
                    {
                        Occurrences =
                        {
                            today.AddDays(-30),
                            today.AddDays(-10)
                        }
                    },
                    new Memory()
                    {
                        Occurrences =
                        {
                            today.AddDays(-11),
                            today.AddDays(-5)
                        }
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.First(s => s.Description == "most days without cringe").Number.Should().Be
                    .EqualTo(19);
            }
        }
    }
}