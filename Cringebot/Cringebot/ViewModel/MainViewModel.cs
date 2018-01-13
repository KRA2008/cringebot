using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Cringebot.ViewModel
{
    public class MainViewModel : FreshBasePageModel
    {
        public List<Memory> FullListMemories { get; set; }
        [DependsOn(nameof(MemoryInput))]
        public IEnumerable<Memory> DisplayMemories
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(MemoryInput))
                {
                    return FullListMemories.Where(m => m.Description.ToLower().Contains(MemoryInput.ToLower()));
                }
                return FullListMemories;
            }
        }

        private bool _simulate;
        public bool Simulate
        {
            get
            {
                return _simulate;
            }
            set
            {
                _simulate = value;
                _dataStore.Save(SIMULATE_STORE_KEY, value);
            }
        }

        private bool _showList;
        public bool ShowList
        {
            get
            {
                return _showList;
            }
            set
            {
                _showList = value;
                _dataStore.Save(SHOW_LIST_STORE_KEY, value);
            }
        }

        public Command AddMemoryCommand { get; set; }
        public Command AddOccurrenceCommand { get; set; }
        public string MemoryInput { get; set; }

        private const string SIMULATE_STORE_KEY = "simulate";
        private const string SHOW_LIST_STORE_KEY = "showList";
        private const string MEMORY_LIST_STORE_KEY = "memoryList";

        private IAppDataStore _dataStore;

        public MainViewModel(IAppDataStore dataStore)
        {
            _dataStore = dataStore;
            FullListMemories = new List<Memory>();

            AddMemoryCommand = new Command(() =>
            {
                if(!string.IsNullOrWhiteSpace(MemoryInput))
                {
                    FullListMemories.Add(new Memory
                    {
                        Description = MemoryInput
                    });
                    MemoryInput = null;

                    _dataStore.Save(MEMORY_LIST_STORE_KEY, FullListMemories);
                }
            });

            AddOccurrenceCommand = new Command((arg) => {
                var memory = (Memory)arg;
                memory.Occurrences.Add(DateTime.Now);
                
                _dataStore.Save(MEMORY_LIST_STORE_KEY, FullListMemories);
            });

            PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case (nameof(Simulate)):
                        _dataStore.Save(SIMULATE_STORE_KEY, Simulate);
                        break;
                    case (nameof(ShowList)):
                        _dataStore.Save(SHOW_LIST_STORE_KEY, ShowList);
                        break;
                }
            };
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            if (_dataStore.TryLoad(SIMULATE_STORE_KEY, out bool storedSimulate))
            {
                Simulate = storedSimulate;
            }
            else
            {
                Simulate = true;
            }

            if (_dataStore.TryLoad(SHOW_LIST_STORE_KEY, out bool storedShowList))
            {
                ShowList = storedShowList;
            }
            else
            {
                ShowList = true;
            }

            if(_dataStore.TryLoad(MEMORY_LIST_STORE_KEY, out List<Memory> storedList))
            {
                FullListMemories = storedList;
            }
        }
    }
}