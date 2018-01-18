using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;
using PropertyChanged;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Cringebot.PageModel
{
    public class MainPageModel : FreshBasePageModel
    {
        private List<Memory> _fullListMemories;
        [DependsOn(nameof(MemoryInput), nameof(_fullListMemories))]
        public IEnumerable<Memory> Memories
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(MemoryInput))
                {
                    return _fullListMemories.Where(m => m.Description.ToLower().Contains(MemoryInput.ToLower()));
                }
                return _fullListMemories;
            }
        }

        public bool Simulate { get; set; }
        public bool ShowList { get; set; }
        public string MemoryInput { get; set; }

        public Command AddMemoryCommand { get; }
        public Command AddOccurrenceCommand { get; }
        public Command ViewDetailsCommand { get; }

        private IAppDataStore _dataStore;

        public MainPageModel(IAppDataStore dataStore)
        {
            _dataStore = dataStore;

            AddMemoryCommand = new Command(() =>
            {
                if(!string.IsNullOrWhiteSpace(MemoryInput))
                {
                    var newMemory = new Memory
                    {
                        Description = MemoryInput
                    };
                    newMemory.Occurrences.Add(SystemTime.Now());
                    _fullListMemories.Add(newMemory);

                    _fullListMemories = _fullListMemories.OrderBy(m => m.Description).ToList();

                    MemoryInput = null;
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
        }

        public async Task ViewDetails(Memory memory) //grrrrr, switch to AsyncCommand
        {
            await CoreMethods.PushPageModel<DetailsPageModel>(memory);
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Simulate = _dataStore.LoadOrDefault(StorageWrapper.SIMULATE_STORE_KEY, true);
            ShowList = _dataStore.LoadOrDefault(StorageWrapper.SHOW_LIST_STORE_KEY, true);
            _fullListMemories = _dataStore.LoadOrDefault(StorageWrapper.MEMORY_LIST_STORE_KEY, new List<Memory>());
        }

        public void Save()
        {
            _dataStore.Save(StorageWrapper.SHOW_LIST_STORE_KEY, ShowList);
            _dataStore.Save(StorageWrapper.SIMULATE_STORE_KEY, Simulate);
            _dataStore.Save(StorageWrapper.MEMORY_LIST_STORE_KEY, _fullListMemories);
        }
    }
}