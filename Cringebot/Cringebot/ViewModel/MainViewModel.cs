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

        public bool Simulate { get; set; }
        public bool LimitListVisibility { get; set; }
        public string MemoryInput { get; set; }

        public Command AddMemoryCommand { get; }
        public Command AddOccurrenceCommand { get; }
        public Command ViewDetailsCommand { get; }
        public Command ViewGraphCommand { get; }
        public Command ViewHelpCommand { get; }
        public Command ViewSettingsCommand { get; }

        private Settings _settings;

        private readonly IAppDataStore _dataStore;
        private readonly INotificationManager _notificationManager;

        public MainViewModel(IAppDataStore dataStore, INotificationManager notificationManager,
            IKeyboardHelper keyboardHelper)
        {
            _dataStore = dataStore;
            _notificationManager = notificationManager;
            _memories = new List<Memory>();

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
            });

            AddOccurrenceCommand = new Command(arg => 
            {
                var memory = (Memory)arg;
                memory.Occurrences.Insert(0, SystemTime.Now());
                MemoryInput = "";
                keyboardHelper.HideKeyboard();
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

            _notificationManager.SetMemories(_memories);
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Simulate = _dataStore.LoadOrDefault(StorageWrapper.SIMULATE_STORE_KEY, false);
            LimitListVisibility = _dataStore.LoadOrDefault(StorageWrapper.LIMIT_LIST_STORE_KEY, false);
            _memories = _dataStore.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, new List<Memory>());
            _settings = _dataStore.LoadOrDefault(StorageWrapper.SETTINGS_STORE_KEY, new Settings());

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

        public void Save()
        {
            _dataStore.Save(StorageWrapper.LIMIT_LIST_STORE_KEY, LimitListVisibility);
            _dataStore.Save(StorageWrapper.SIMULATE_STORE_KEY, Simulate);
            _dataStore.Save(StorageWrapper.MEMORY_LIST_STORE_KEY, _memories);
            _dataStore.Save(StorageWrapper.SETTINGS_STORE_KEY, _settings);
        }
    }
}