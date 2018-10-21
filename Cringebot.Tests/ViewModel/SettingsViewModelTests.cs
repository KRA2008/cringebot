using System;
using System.Linq;
using System.Threading.Tasks;
using Cringebot.Model;
using Cringebot.Services;
using Cringebot.ViewModel;
using Cringebot.Wrappers;
using FreshMvvm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.ViewModel
{
    public class SettingsViewModelTests
    {
        private SettingsViewModel _viewModel;
        private SettingsPushPackage _pushPackage;
        private Settings _settings;
        private Mock<INotificationManager> _notificationManager;
        private Mock<IThemeService> _themeService;

        private const double RAPID_FIRE_TIME_HOURS = 0.005;

        [SetUp]
        public void InstantiateViewModel()
        {
            _settings = new Settings();
            _pushPackage = new SettingsPushPackage {Settings = _settings};
            _notificationManager = new Mock<INotificationManager>();
            _themeService = new Mock<IThemeService>();
            _viewModel = new SettingsViewModel(_notificationManager.Object, _themeService.Object);
        }

        public sealed class InitMethod : SettingsViewModelTests
        {
            [Test]
            public void ShouldProvideMaxHoursRange()
            {
                //act
                _viewModel.Init(_pushPackage);

                //assert
                _viewModel.MaxHoursChoices.First().Should().Be.EqualTo(RAPID_FIRE_TIME_HOURS);
                _viewModel.MaxHoursChoices.Last().Should().Be.EqualTo(999);
            }

            [Test]
            public void ShouldProvideMinHoursRange()
            {
                //act
                _viewModel.Init(_pushPackage);

                //assert
                _viewModel.MinHoursChoices.First().Should().Be.EqualTo(0);
                _viewModel.MinHoursChoices.Last().Should().Be.EqualTo(999);
            }

            [Test]
            public void ShouldBindData()
            {
                //act
                _viewModel.Init(_pushPackage);

                //assert
                _viewModel.Settings.Should().Be.SameInstanceAs(_settings);
            }

            [Test]
            public void ShouldInitializeTimePropsToConvertedSettings()
            {
                //arrange
                _settings.GenerationMaxInterval = new TimeSpan(4, 1, 15, 18);
                _settings.GenerationMinInterval = new TimeSpan(3, 5, 0, 0);

                //act
                _viewModel.Init(_pushPackage);

                //assert
                _viewModel.Satisfies(vm =>
                        vm.MaxHours == 97.255 &&
                        vm.MinHours == 77)
                    .Should().Be.True();
            }

            [Test]
            public void ShouldSetThemes()
            {
                //arrange
                const string EXPECTED_THEME_1 = "hi";
                const string EXPECTED_THEME_2 = "hello";
                _themeService.Setup(t => t.GetThemes()).Returns(new[]
                {
                    EXPECTED_THEME_1,
                    EXPECTED_THEME_2
                });

                //act
                _viewModel.Init(_pushPackage);

                //assert
                _viewModel.ThemeNames.Should().Have.SameSequenceAs(EXPECTED_THEME_1, EXPECTED_THEME_2);
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
                _viewModel.Init(_pushPackage);

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
                _viewModel.Init(_pushPackage);

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
                _viewModel.Init(_pushPackage);
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
                _viewModel.Init(_pushPackage);

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
                _viewModel.Init(_pushPackage);

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
                _viewModel.Init(_pushPackage);
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
                _viewModel.Init(_pushPackage);
                _viewModel.MinHours = 0;
                _viewModel.MaxHours = RAPID_FIRE_TIME_HOURS;

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
                    s.GenerationMaxInterval.Minutes == 0 &&
                    s.GenerationMaxInterval.Seconds == 18 &&
                    s.GenerationMinInterval.Days == 0 &&
                    s.GenerationMinInterval.Hours == 0);
            }
        }

        public sealed class SetThemeCommandProperty : SettingsViewModelTests
        {
            [Test]
            public void ShouldSetTheme()
            {
                //arrange
                const string EXPECTED_THEME = "whatever!";

                //act
                _viewModel.SetTheme.Execute(EXPECTED_THEME);

                //assert
                _themeService.Verify(t => t.ApplyTheme(EXPECTED_THEME));
            }
        }

        public sealed class ViewImportExportCommandProperty : SettingsViewModelTests
        {
            [Test]
            public async Task ShouldNavigateToImportExportPage()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;

                //act
                await _viewModel.NavigateToImportExportAsync();

                //assert
                coreMethods.Verify(c => c.PushPageModel<ImportExportViewModel>(null, false, true));
            }
        }

        public sealed class PopBackMethod : SettingsViewModelTests
        {
            [Test]
            public async Task ShouldPopBackWithWhateverIsPassedIn()
            {
                //arrange
                var coreMethods = new Mock<IPageModelCoreMethods>();
                _viewModel.CoreMethods = coreMethods.Object;
                var randomThing = new object();

                //act
                await _viewModel.PopBack(randomThing);

                //assert
                coreMethods.Verify(c => c.PopPageModel(randomThing, false, true));
            }
        }
    }
}