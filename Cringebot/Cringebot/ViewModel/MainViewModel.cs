using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System;
using System.Linq;
using System.Net;
using Cringebot.Services;

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

        public Command AddMemoryCommand { get; }
        public Command AddOccurrenceCommand { get; }
        public Command ViewDetailsCommand { get; }
        public Command ViewGraphCommand { get; }
        public Command ViewHelpCommand { get; }
        public Command ViewSettingsCommand { get; }

        private Settings _settings;

        private readonly IAppProperties _properties;
        private readonly INotificationManager _notificationManager;

        public MainViewModel(IAppProperties properties, INotificationManager notificationManager,
            IKeyboardHelper keyboardHelper)
        {
            _properties = properties;
            _notificationManager = notificationManager;
            _memories = new List<Memory>();

            MessagingCenter.Subscribe<ThemeService,bool>(this, ThemeService.TOOLS_SHOULD_BE_BLACK_CHANGED, SetToolbarIcons);

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

                notificationManager.SetMemories(_memories);

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

            ViewDetailsCommand = new Command(async args => 
            {
                var memory = (Memory)((ItemTappedEventArgs)args).Item;
                await ViewDetails(memory);
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
                    notificationManager.StartNotifications(_memories, _settings);
                }
                else
                {
                    notificationManager.StopNotifications();
                }
            };
        }

        private void SetToolbarIcons(ThemeService obj, bool blackItems)
        {
            while (CurrentPage.ToolbarItems.Count > 0)
            {
                CurrentPage.ToolbarItems.RemoveAt(0);
            }

            var colorSuffix = Device.RuntimePlatform == Device.Android && blackItems ? "black" : "";

            CurrentPage.ToolbarItems.Add(new ToolbarItem
            {
                Icon = "gear"+colorSuffix,
                Command = ViewSettingsCommand
            });
            CurrentPage.ToolbarItems.Add(new ToolbarItem
            {
                Icon = "help" + colorSuffix,
                Command = ViewHelpCommand
            });
            CurrentPage.ToolbarItems.Add(new ToolbarItem
            {
                Icon = "chart" + colorSuffix,
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
            await CoreMethods.PushPageModel<SettingsViewModel>(_settings);
        }

        public override void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);

            var memoryToRemove = (Memory)returnedData;
            _memories.Remove(memoryToRemove);
            RaisePropertyChanged(nameof(Memories));
            RaisePropertyChanged(nameof(SearchResultCount));

            _notificationManager.SetMemories(_memories);
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Simulate = _properties.LoadOrDefault(PropertiesWrapper.SIMULATE_STORE_KEY, false);
            LimitListVisibility = _properties.LoadOrDefault(PropertiesWrapper.LIMIT_LIST_STORE_KEY, false);
            _memories = _properties.LoadOrDefault(PropertiesWrapper.MEMORY_LIST_STORE_KEY, new List<Memory>());
            _settings = _properties.LoadOrDefault(PropertiesWrapper.SETTINGS_STORE_KEY, new Settings());

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
            _properties.Save(PropertiesWrapper.LIMIT_LIST_STORE_KEY, LimitListVisibility);
            _properties.Save(PropertiesWrapper.SIMULATE_STORE_KEY, Simulate);
            _properties.Save(PropertiesWrapper.MEMORY_LIST_STORE_KEY, _memories);
            _properties.Save(PropertiesWrapper.SETTINGS_STORE_KEY, _settings);
        }
    }
}