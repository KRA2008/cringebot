using Cringebot.Model;
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

        public MainViewModel()
        {
            FullListMemories = new List<Memory>();

            Simulate = true;
            ShowList = true;

            AddMemoryCommand = new Command(() =>
            {
                FullListMemories.Add(new Memory
                {
                    Description = MemoryInput
                });
                MemoryInput = null;
            });
        }
    }
}