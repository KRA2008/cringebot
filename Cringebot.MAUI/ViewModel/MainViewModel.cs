using System;
using System.Collections.Generic;
using Cringebot.Model;
using Cringebot.Wrappers;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cringebot.Services;
using FreshMvvm.Maui;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Controls;

namespace Cringebot.ViewModel
{
    public class MainViewModel : FreshBasePageModel
    {
        [DependsOn(nameof(MemoryInput))]
        private List<Memory> _memories;
        public ObservableCollection<Memory> Memories
        {
            get
            {
                Predicate<Memory> filterPredicate = a => true;
                if(!string.IsNullOrWhiteSpace(MemoryInput))
                {
                    filterPredicate = a => a.Description.ToLower().Contains(MemoryInput.ToLower());
                }
                var filtered = _memories.Where(m => filterPredicate(m)).ToList();
                if (LimitListVisibility)
                {
                    if(filtered.Count > 1 || string.IsNullOrWhiteSpace(MemoryInput) || MemoryInput.Length < 3)
                    {
                        filtered = filtered.Where(m => false).ToList();
                    }
                }
                return new ObservableCollection<Memory>(filtered.OrderBy(m => m.Description));
            }
        }
        
        public int SearchResultCount
        {
            get
            {
                return !string.IsNullOrEmpty(MemoryInput)
                    ? _memories.Count(m => m.Description.ToLower().Contains(MemoryInput.ToLower()))
                    : _memories.Count;
            }
        }

        public bool Simulate { get; set; }
        public bool LimitListVisibility { get; set; }
        public string MemoryInput { get; set; }

        public bool CringeFlashTrigger { get; set; }

        public Memory SelectedMemory { get; set; }

        public Command AddMemoryCommand { get; }
        public Command AddOccurrenceCommand { get; }
        public Command ViewDetailsCommand { get; }
        public Command ViewGraphCommand { get; }
        public Command ViewHelpCommand { get; }
        public Command ViewSettingsCommand { get; }

        private Settings _settings;

        private readonly IPersistentStorage _properties;
        private readonly INotificationManager _notificationManager;

        public MainViewModel(IPersistentStorage persistentStorage, INotificationManager notificationManager,
            IKeyboardHelper keyboardHelper)
        {
            _properties = persistentStorage;
            _notificationManager = notificationManager;
            _memories = new List<Memory>();

            MessagingCenter.Subscribe<ThemeService>(this, ThemeService.THEME_SET_MESSAGE, SetToolbarIcons);

            AddMemoryCommand = new Command(() =>
            {
                if (string.IsNullOrWhiteSpace(MemoryInput)) return;

                var newMemory = new Memory
                {
                    Description = MemoryInput
                };
                newMemory.Occurrences.Add(SystemTime.Now());
                _memories.Add(newMemory);
                RaisePropertyChanged(nameof(Memories));

                MemoryInput = null;

                _notificationManager.SetMemories(_memories);

                CringeFlashTrigger = !CringeFlashTrigger;
            });

            AddOccurrenceCommand = new Command(arg => 
            {
                var memory = (Memory)arg;
                memory.Occurrences.Insert(0, SystemTime.Now());
                MemoryInput = "";
                keyboardHelper.HideKeyboard();
                CringeFlashTrigger = !CringeFlashTrigger;
            });

            ViewDetailsCommand = new Command(async () =>
            {
                if (SelectedMemory == null) return;
                var tapped = SelectedMemory;
                SelectedMemory = null;
                await ViewDetails(tapped);
            });

            ViewGraphCommand = new Command(async args =>
            {
                await ViewGraph(_memories);
            });

            ViewHelpCommand = new Command(async args =>
            {
                await ViewHelp();
            });

            ViewSettingsCommand = new Command(async args =>
            {
                await ViewSettings();
            });

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(Simulate)) return;

                if(Simulate)
                {
                    _notificationManager.StartNotifications(_memories, _settings);
                    ShowSimulationExplanation();
                }
                else
                {
                    _notificationManager.StopNotifications();
                }
            };
        }

        private async void ShowSimulationExplanation()
        {
            await CoreMethods.DisplayAlert(
                "Simulation Mode Activated",
                "Cringebot will now simulate the experience of involuntary recall by sending you notifications about items in the list at random intervals.",
                "OK");
        }

        private void SetToolbarIcons(ThemeService obj)
        {

            CurrentPage.ToolbarItems.Clear();

            var colorSuffix = Device.RuntimePlatform == Device.Android && ThemeService.ToolsShouldBeBlack ? "black" : "";

            CurrentPage.ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = "gear" + colorSuffix + ".png",
                Command = ViewSettingsCommand
            });
            CurrentPage.ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = "help" + colorSuffix + ".png",
                Command = ViewHelpCommand
            });
            CurrentPage.ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = "chart" + colorSuffix + ".png",
                Command = ViewGraphCommand
            });
        }

        public async Task ViewDetails(Memory memory) //grrrrr, switch to AsyncCommand
        {
            await CoreMethods.PushPageModel<DetailsViewModel>(memory);
        }

        public async Task ViewGraph(IEnumerable<Memory> memories)
        {
            await CoreMethods.PushPageModel<ChartViewModel>(memories);
        }

        public async Task ViewHelp()
        {
            await CoreMethods.PushPageModel<HelpViewModel>();
        }

        public async Task ViewSettings()
        {
            await CoreMethods.PushPageModel<SettingsViewModel>(
                new SettingsPushPackage
                {
                    Settings = _settings,
                    Memories = _memories
                });
        }

        public override void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);

            if (returnedData is SettingsPushPackage pushPackage)
            {
                _memories = pushPackage.Memories;
            }
            else
            {
                var memoryToRemove = (Memory)returnedData;
                _memories.Remove(memoryToRemove);
            }

            RaisePropertyChanged(nameof(Memories));
            RaisePropertyChanged(nameof(SearchResultCount));
            _notificationManager.SetMemories(_memories);
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Simulate = _properties.LoadOrDefault(PersistentStorage.SIMULATE_STORE_KEY, false);
            LimitListVisibility = _properties.LoadOrDefault(PersistentStorage.LIMIT_LIST_STORE_KEY, false);
            _memories = _properties.LoadOrDefault(PersistentStorage.MEMORY_LIST_STORE_KEY, new List<Memory>());
            _settings = _properties.LoadOrDefault(PersistentStorage.SETTINGS_STORE_KEY, new Settings());

            if (Simulate)
            {
                _notificationManager.StartNotifications(_memories, _settings);
            }
            else
            {
                _notificationManager.SetSettings(_settings);
                _notificationManager.SetMemories(_memories);
            }
        }

        public void Import(string import)
        {
            var lines = WebUtility.UrlDecode(import).Split('\n');
            foreach (var line in lines)
            {
                _memories.Add(new Memory
                {
                    Description = line
                });
            }
            RaisePropertyChanged(nameof(Memories));

            _notificationManager.SetMemories(_memories);
        }

        public void Save()
        {
            _properties.Save(PersistentStorage.LIMIT_LIST_STORE_KEY, LimitListVisibility);
            _properties.Save(PersistentStorage.SIMULATE_STORE_KEY, Simulate);
            _properties.Save(PersistentStorage.MEMORY_LIST_STORE_KEY, _memories);
            _properties.Save(PersistentStorage.SETTINGS_STORE_KEY, _settings);
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            try
            {
                base.ViewIsAppearing(sender, e);
                await Task.Delay(100);
                await ViewIsAppearing();
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }

        public async Task ViewIsAppearing() // for testing
        {
            var openedBefore = _properties.LoadOrDefault(PersistentStorage.HAS_OPENED_BEFORE, false);
            if (!openedBefore)
            {
                await CoreMethods.PushPageModel<HelpViewModel>(true,false);
                _properties.Save(PersistentStorage.HAS_OPENED_BEFORE, true);
            }
        }
    }
}