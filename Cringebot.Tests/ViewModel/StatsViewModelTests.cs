using System;
using System.Collections.Generic;
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
            public void ShouldNotDieWhenEmptyList()
            {
                _viewModel.Init(new List<Memory>());
            }

            [Test]
            public void ShouldNotDieWhenNoOccurrences()
            {
                _viewModel.Init(new List<Memory>{ new Memory()});
            }

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
                _viewModel.Statistics.First(s => s.Description == "total cringes").Value.Should().Be.EqualTo("3");
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
                _viewModel.Statistics.First(s => s.Description == "total days on Cringebot").Value.Should().Be
                    .EqualTo((DAYS_SINCE+1).ToString());
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
                _viewModel.Statistics.First(s => s.Description == "average cringes per day").Value.Should().Be
                    .EqualTo("0.2");
            }

            [Test]
            public void ShouldGetMostCringesInSingleDay()
            {
                //arrange
                var memories = new[]
                {
                    new Memory
                    {
                        Occurrences = {
                            new DateTime(1,1,1,1,1,1,1),
                            new DateTime(2,2,2,2,2,2,2),
                            new DateTime(3,3,3,3,3,3,3)}
                    },
                    new Memory
                    {
                        Occurrences =
                        {
                            new DateTime(2,2,2,2,2,2,2),
                            new DateTime(1,1,1,1,1,1,1),
                            new DateTime(4,4,4,4,4,4,4),
                            new DateTime(1,1,1,1,1,1,1)
                        }
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.First(s => s.Description == "most cringes in single day").Value.Should().Be
                    .EqualTo("3");
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
                _viewModel.Statistics.First(s => s.Description == "total days without cringe").Value.Should().Be
                    .EqualTo("28");
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
                    new Memory
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
                _viewModel.Statistics.First(s => s.Description == "most days without cringe").Value.Should().Be
                    .EqualTo("19");
            }

            [Test]
            public void ShouldGetZeroForLongestRunWithoutCringeWhenOneOccurrenceTotal()
            {
                //arrange
                var memory = new Memory();
                memory.Occurrences.Add(new DateTime());

                var memories = new[]
                {
                    memory
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.First(s => s.Description == "most days without cringe").Value.Should().Be
                    .EqualTo("0");
            }

            [Test]
            public void ShouldGetCringiestMemory()
            {
                //arrange
                const string EXPECTED_CRINGIEST_DESCRIPTION = "the worst";
                var memories = new[]
                {
                    new Memory
                    {
                        Description = "not bad",
                        Occurrences = { new DateTime()}
                    },
                    new Memory
                    {
                        Description = EXPECTED_CRINGIEST_DESCRIPTION,
                        Occurrences = { new DateTime(), new DateTime()}
                    },
                    new Memory
                    {
                        Description = "easy"
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.First(s => s.Description == "cringiest").Value.Should().Be
                    .EqualTo(EXPECTED_CRINGIEST_DESCRIPTION);
            }

            [Test]
            public void ShouldExcludeCringiestIfOnlyOneMemory()
            {
                //arrange
                var memories = new[]
                {
                    new Memory
                    {
                        Description = "not bad",
                        Occurrences = { new DateTime()}
                    }
                };

                //act
                _viewModel.Init(memories);

                //assert
                _viewModel.Statistics.FirstOrDefault(s => s.Description == "cringiest").Should().Be.Null();
            }

            [Test]
            public void ShouldSetTitleToSingleMemoryWhenSingle()
            {
                //arrange
                const string DESC = "hello there";

                //act
                _viewModel.Init(new[] {new Memory
                {
                    Description = DESC
                }});

                //assert
                _viewModel.Title.Should().Be.EqualTo("Stats (" + DESC + ")");
            }

            [Test]
            public void ShouldSetTitleToGlobalWhenGlobal()
            {
                //act
                _viewModel.Init(new[] {
                    new Memory(),
                    new Memory()
                });

                //assert
                _viewModel.Title.Should().Be.EqualTo("Stats");
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
    }
}