using Cringebot.Model;
using FreshMvvm;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Cringebot.ViewModel
{
    public class MainViewModel : FreshBasePageModel
    {
        public ObservableCollection<Memory> Memories { get; set; }
        public bool Simulate { get; set; }
        public Command AddMemoryCommand { get; set; }
        public string MemoryInput { get; set; }

        public MainViewModel()
        {
            Memories = new ObservableCollection<Memory>();

            Simulate = true;

            AddMemoryCommand = new Command(() =>
            {
                Memories.Add(new Memory
                {
                    Description = MemoryInput
                });
                MemoryInput = null;
            });
        }
    }
}