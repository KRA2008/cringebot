using System;
using System.Linq;
using Cringebot.Model;
using Cringebot.ViewModel;
using Cringebot.Wrappers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.ViewModel
{
    public class SettingsViewModelTests
    {
        private SettingsViewModel _viewModel;
        private Settings _settings;
        private Mock<INotificationManager> _notificationManager;

        private const decimal RAPID_FIRE_TIME_HOURS = 0.02m;

        [SetUp]
        public void InstantiateViewModel()
        {
            _settings = new Settings();
            _notificationManager = new Mock<INotificationManager>();
            _viewModel = new SettingsViewModel(_notificationManager.Object);
        }

        public sealed class InitMethod : SettingsViewModelTests
        {
            [Test]
            public void ShouldProvideMaxHoursRange()
            {
                //act
                _viewModel.Init(_settings);

                //assert
                _viewModel.MaxHoursChoices.First().Should().Be.EqualTo(0.02);
                _viewModel.MaxHoursChoices.Last().Should().Be.EqualTo(99);
            }

            [Test]
            public void ShouldProvideMinHoursRange()
            {
                //act
                _viewModel.Init(_settings);

                //assert
                _viewModel.MinHoursChoices.First().Should().Be.EqualTo(0);
                _viewModel.MinHoursChoices.Last().Should().Be.EqualTo(99);
            }

            [Test]
            public void ShouldBindData()
            {
                //act
                _viewModel.Init(_settings);

                //assert
                _viewModel.Settings.Should().Be.SameInstanceAs(_settings);
            }

            [Test]
            public void ShouldInitializeTimePropsToConvertedSettings()
            {
                //arrange
                _settings.GenerationMaxInterval = new TimeSpan(4, 1, 15, 0);
                _settings.GenerationMinInterval = new TimeSpan(3, 5, 0, 0);

                //act
                _viewModel.Init(_settings);

                //assert
                _viewModel.Satisfies(vm =>
                        vm.MaxHours == 97.25m &&
                        vm.MinHours == 77)
                    .Should().Be.True();
            }
        }

        public sealed class MaxHoursProperty : SettingsViewModelTests
        {
            [Test]
            public void ShouldNotBeAllowedToGoBelowMin()
            {
                //arrange
                _settings.GenerationMaxInterval = new TimeSpan(5, 0, 0);
                _settings.GenerationMinInterval = new TimeSpan(4, 0, 0);
                _viewModel.Init(_settings);

                //act
                _viewModel.MaxHours = 3;

                //assert
                _viewModel.Satisfies(vm =>
                    vm.MaxHours == 4 &&
                    vm.MinHours == 4).Should().Be.True();
            }

            [Test]
            public void ShouldRaisePropertyChangedForExplanationText()
            {
                //arrange
                var called = false;
                _viewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(_viewModel.GenerationIntervalExplanation))
                    {
                        called = true;
                    }
                };

                //act
                _viewModel.MaxHours = 4;

                //assert
                called.Should().Be.True();
            }
        }

        public sealed class MinHoursProperty : SettingsViewModelTests
        {
            [Test]
            public void ShouldNotBeAllowedToGoAboveMax()
            {
                //arrange
                _settings.GenerationMaxInterval = new TimeSpan(5, 0, 0);
                _settings.GenerationMinInterval = new TimeSpan(0, 0, 0);
                _viewModel.Init(_settings);

                //act
                _viewModel.MinHours = 6;

                //assert
                _viewModel.Satisfies(vm =>
                    vm.MaxHours == 5 &&
                    vm.MinHours == 5).Should().Be.True();
            }

            [Test]
            public void ShouldBeSetToZeroIfTryingToExceedMaxButMaxIsAtRapidFire()
            {
                //arrange
                _settings.GenerationMaxInterval = new TimeSpan(5, 0, 0);
                _settings.GenerationMinInterval = new TimeSpan(0, 0, 0);
                _viewModel.Init(_settings);
                _viewModel.MaxHours = RAPID_FIRE_TIME_HOURS;

                //act
                _viewModel.MinHours = 6;

                //assert
                _viewModel.Satisfies(vm =>
                    vm.MaxHours == RAPID_FIRE_TIME_HOURS &&
                    vm.MinHours == 0).Should().Be.True();
            }

            [Test]
            public void ShouldRaisePropertyChangedForExplanationText()
            {
                //arrange
                var called = false;
                _viewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(_viewModel.GenerationIntervalExplanation))
                    {
                        called = true;
                    }
                };

                //act
                _viewModel.MinHours = 4;

                //assert
                called.Should().Be.True();
            }
        }

        public sealed class SettingsProperty : SettingsViewModelTests
        {
            [Test]
            public void ShouldRaisePropertyChangedOnExplanationMessageWhenDoNotDisturbStartChanges()
            {
                _viewModel.Init(_settings);

                var called = false;
                _viewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(_viewModel.DoNotDisturbExplanation))
                    {
                        called = true;
                    }
                };

                //act
                _viewModel.Settings.DoNotDisturbStartTime = new TimeSpan();

                //assert
                called.Should().Be.True();
            }

            [Test]
            public void ShouldRaisePropertyChangedOnExplanationMessageWhenDoNotDisturbStopChanges()
            {
                _viewModel.Init(_settings);

                var called = false;
                _viewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(_viewModel.DoNotDisturbExplanation))
                    {
                        called = true;
                    }
                };

                //act
                _viewModel.Settings.DoNotDisturbStopTime = new TimeSpan();

                //assert
                called.Should().Be.True();
            }
        }

        public sealed class SaveSettingsMethod : SettingsViewModelTests
        {
            [Test]
            public void ShouldConvertAndSaveSettings()
            {
                //arrange
                _viewModel.Init(_settings);
                _viewModel.MaxHours = 50;
                _viewModel.MinHours = 10;

                Settings savedSettings = null;
                _notificationManager.Setup(m => m.SetSettings(It.IsAny<Settings>())).Callback<Settings>(s =>
                    {
                        savedSettings = s;
                    });

                //act
                _viewModel.SaveSettings();

                //assert
                savedSettings.Satisfies(s => 
                    s.GenerationMaxInterval.Days == 2 &&
                    s.GenerationMaxInterval.Hours == 2 &&
                    s.GenerationMinInterval.Days == 0 &&
                    s.GenerationMinInterval.Hours == 10);
            }

            [Test]
            public void ShouldConvertAndSaveSettingsWhenMaxFractional()
            {
                //arrange
                _viewModel.Init(_settings);
                _viewModel.MinHours = 0;
                _viewModel.MaxHours = 0.02m;

                Settings savedSettings = null;
                _notificationManager.Setup(m => m.SetSettings(It.IsAny<Settings>())).Callback<Settings>(s =>
                {
                    savedSettings = s;
                });

                //act
                _viewModel.SaveSettings();

                //assert
                savedSettings.Satisfies(s =>
                    s.GenerationMaxInterval.Days == 0 &&
                    s.GenerationMaxInterval.Hours == 0 &&
                    s.GenerationMaxInterval.Minutes == 1 &&
                    s.GenerationMinInterval.Days == 0 &&
                    s.GenerationMinInterval.Hours == 0);
            }
        }
    }
}