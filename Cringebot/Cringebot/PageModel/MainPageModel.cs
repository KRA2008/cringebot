﻿using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Cringebot.Page.CustomElements;
using System;
using System.Linq;

namespace Cringebot.PageModel
{
    public class MainPageModel : FreshBasePageModel
    {
        private List<Memory> _filteredOutMemories;
        [DependsOn(nameof(MemoryInput), nameof(_filteredOutMemories))]
        private ObservableCollection<Memory> _memories;
        public ObservableCollection<Memory> Memories
        {
            get
            {
                Predicate<Memory> filterPredicate = delegate (Memory a) { return true; };
                if(!string.IsNullOrWhiteSpace(MemoryInput))
                {
                    filterPredicate = delegate (Memory a) { return a.Description.ToLower().Contains(MemoryInput.ToLower()); };
                }
                _memories.FilterButPreserve(_filteredOutMemories, filterPredicate);
                if (LimitListVisibility)
                {
                    if(_memories.Count() > 1 || string.IsNullOrWhiteSpace(MemoryInput) || MemoryInput.Length < 3)
                    {
                        _memories.FilterButPreserve(_filteredOutMemories, delegate (Memory a) { return false; });
                    }
                }
                _memories.Sort((a, b) => { return a.Description.CompareTo(b.Description); });
                return _memories;
            }
            set
            {
                _memories = value;
            }
        }

        public bool Simulate { get; set; }
        public bool LimitListVisibility { get; set; }
        public string MemoryInput { get; set; }

        public Command AddMemoryCommand { get; }
        public Command AddOccurrenceCommand { get; }
        public Command ViewDetailsCommand { get; }

        private IAppDataStore _dataStore;
        private INotificationManager _notificationManager;

        public MainPageModel(IAppDataStore dataStore, INotificationManager notificationManager)
        {
            _dataStore = dataStore;
            _notificationManager = notificationManager;
            _filteredOutMemories = new List<Memory>();
            _memories = new ObservableCollection<Memory>();

            AddMemoryCommand = new Command(() =>
            {
                if(!string.IsNullOrWhiteSpace(MemoryInput))
                {
                    var newMemory = new Memory
                    {
                        Description = MemoryInput
                    };
                    newMemory.Occurrences.Add(SystemTime.Now());
                    _filteredOutMemories.Add(newMemory);

                    MemoryInput = null;

                    notificationManager.SetMemories(_filteredOutMemories.Union(_memories));
                }
            });

            AddOccurrenceCommand = new Command((arg) => 
            {
                var memory = (Memory)arg;
                memory.Occurrences.Add(SystemTime.Now());
            });

            ViewDetailsCommand = new Command(async(args) => 
            {
                var memory = (Memory)((ItemTappedEventArgs)args).Item;
                await ViewDetails(memory);
            });

            PropertyChanged += (sender, args) =>
            {
                if(args.PropertyName == nameof(Simulate))
                {
                    if(Simulate)
                    {
                        notificationManager.StartNotifications();
                    }
                    else
                    {
                        notificationManager.StopNotifications();
                    }
                }
            };
        }

        public async Task ViewDetails(Memory memory) //grrrrr, switch to AsyncCommand
        {
            await CoreMethods.PushPageModel<DetailsPageModel>(memory);
        }

        public override void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);

            var memoryToRemove = (Memory)returnedData;
            _filteredOutMemories.Remove(memoryToRemove);
            _memories.Remove(memoryToRemove);

            _notificationManager.SetMemories(_filteredOutMemories.Union(_memories));
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Simulate = _dataStore.LoadOrDefault(StorageWrapper.SIMULATE_STORE_KEY, false);
            LimitListVisibility = _dataStore.LoadOrDefault(StorageWrapper.LIMIT_LIST_STORE_KEY, false);
            _filteredOutMemories = _dataStore.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, new List<Memory>());

            _notificationManager.SetMemories(_filteredOutMemories.Union(_memories));
        }

        public void Save()
        {
            _dataStore.Save(StorageWrapper.LIMIT_LIST_STORE_KEY, LimitListVisibility);
            _dataStore.Save(StorageWrapper.SIMULATE_STORE_KEY, Simulate);
            _dataStore.Save(StorageWrapper.MEMORY_LIST_STORE_KEY, _filteredOutMemories.Union(_memories));
        }
    }
}