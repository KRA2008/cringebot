using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;
using PropertyChanged;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Cringebot.ViewModel
{
    public class MainViewModel : FreshBasePageModel
    {
        public List<Memory> FullListMemories { get; set; }
        [DependsOn(nameof(MemoryInput), nameof(ShowList))]
        public IEnumerable<Memory> DisplayMemories
        {
            get
            {
                if(!ShowList)
                {
                    return Enumerable.Empty<Memory>();
                }
                if(!string.IsNullOrWhiteSpace(MemoryInput))
                {
                    return FullListMemories.Where(m => m.Description.Contains(MemoryInput));
                }
                return FullListMemories;
            }
        }
        public bool Simulate { get; set; }
        public bool ShowList { get; set; }
        public Command AddMemoryCommand { get; set; }
        public string MemoryInput { get; set; }

        private const string SIMULATE_STORE_KEY = "simulate";
        private const string SHOW_LIST_STORE_KEY = "showList";

        private IAppDataStore _dataStore;

        public MainViewModel(IAppDataStore dataStore)
        {
            _dataStore = dataStore;
            FullListMemories = new List<Memory>();

            AddMemoryCommand = new Command(() =>
            {
                FullListMemories.Add(new Memory
                {
                    Description = MemoryInput
                });
                MemoryInput = null;
            });
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
        }
    }
}